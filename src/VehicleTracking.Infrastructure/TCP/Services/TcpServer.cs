using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.GeoJsonObjectModel;
using Polly;
using VehicleTracking.Application.Services;
using VehicleTracking.Domain.Repositories;
using VehicleTracking.Infrastructure.Common;
using VehicleTracking.Infrastructure.Data.BulkUpdates;
using VehicleTracking.Infrastructure.Monitoring;
using VehicleTracking.Infrastructure.TCP.Interfaces;
using VehicleTracking.Infrastructure.TCP.Models;
using VehicleTracking.Infrastructure.Services;

namespace VehicleTracking.Infrastructure.TCP.Services
{
    public class TcpServer : IHostedService, IDisposable
    {
        private readonly ILogger<TcpServer> _logger;
        private readonly ITcpDataProcessor _dataProcessor;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly BatchProcessor<TcpLocationData> _batchProcessor;
        private readonly RetryPolicy _retryPolicy;
        private readonly BulkUpdateService _bulkUpdateService;
        private readonly TcpConnectionPool _connectionPool;
        private readonly PerformanceMetrics _performanceMetrics;
        private readonly IVehicleService _vehicleService;
        private readonly ILocationBroadcastService _locationBroadcastService;
        private readonly IVeraMobilGatewayService _veraMobilGatewayService;
        private Task _serverTask;
        private readonly int _port;
        private readonly Dictionary<string, string> _deviceIdToVehicleIdMap;
        private readonly SemaphoreSlim _mapLock = new SemaphoreSlim(1, 1);
        private int _totalConnectionsReceived = 0;
        private int _activeConnections = 0;
        private int _messagesProcessed = 0;
        private readonly Timer _metricsReportingTimer;

        public TcpServer(
            ILogger<TcpServer> logger,
            ITcpDataProcessor dataProcessor,
            IVehicleRepository vehicleRepository,
            BulkUpdateService bulkUpdateService,
            RetryPolicy retryPolicy,
            PerformanceMetrics performanceMetrics,
            IVehicleService vehicleService,
            ILocationBroadcastService locationBroadcastService,
            ILogger<BatchProcessor<TcpLocationData>> batchProcessorLogger,
            ILogger<TcpConnectionPool> connectionPoolLogger,
            IVeraMobilGatewayService veraMobilGatewayService = null,
            int port = 5000)
        {
            _logger = logger;
            _dataProcessor = dataProcessor;
            _vehicleRepository = vehicleRepository;
            _bulkUpdateService = bulkUpdateService;
            _retryPolicy = retryPolicy;
            _performanceMetrics = performanceMetrics;
            _vehicleService = vehicleService;
            _locationBroadcastService = locationBroadcastService;
            _veraMobilGatewayService = veraMobilGatewayService;
            _port = port;
            _listener = new TcpListener(IPAddress.Any, port);
            _cancellationTokenSource = new CancellationTokenSource();
            _deviceIdToVehicleIdMap = new Dictionary<string, string>();

            // Bağlantı havuzu oluştur
            _connectionPool = new TcpConnectionPool(connectionPoolLogger, 200, 20);

            // Batch processor'ı oluştur ve konum güncellemelerini toplu şekilde işleyecek callback'i tanımla
            _batchProcessor = new BatchProcessor<TcpLocationData>(
                batchProcessorLogger,
                ProcessLocationBatchAsync,
                100,  // maksimum 100 konum bir arada işlenecek
                1000); // maksimum 1 saniye bekleyecek

            // 1 dakikalık aralıklarla bağlantı istatistiklerini logla
            _metricsReportingTimer = new Timer(LogConnectionMetrics, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (_performanceMetrics.MeasureOperation("TcpServer.StartAsync"))
            {
                // Veritabanı nesnelerinin var olduğundan emin ol
                await _bulkUpdateService.EnsureDatabaseObjectsAsync();

                // Aktif araçların device ID'lerini ön belleğe al
                await InitializeDeviceMap();

                // TCP sunucu görevini başlat
                _serverTask = StartServerAsync(_cancellationTokenSource.Token);

                _logger.LogInformation("TCP Server started on port {Port} with performance optimizations and monitoring", _port);
            }

            return;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            using (_performanceMetrics.MeasureOperation("TcpServer.StopAsync"))
            {
                _cancellationTokenSource.Cancel();
                _listener.Stop();

                if (_serverTask != null)
                    await _serverTask;

                _logger.LogInformation("TCP Server stopped");
            }
        }

        private async Task InitializeDeviceMap()
        {
            using (_performanceMetrics.MeasureOperation("TcpServer.InitializeDeviceMap"))
            {
                try
                {
                    var vehicles = await _vehicleRepository.GetActiveVehiclesAsync();

                    await _mapLock.WaitAsync();
                    try
                    {
                        _deviceIdToVehicleIdMap.Clear();
                        foreach (var vehicle in vehicles)
                        {
                            if (!string.IsNullOrEmpty(vehicle.DeviceId))
                            {
                                _deviceIdToVehicleIdMap[vehicle.DeviceId] = vehicle.Id;
                            }
                        }
                    }
                    finally
                    {
                        _mapLock.Release();
                    }

                    _logger.LogInformation("Initialized device-to-vehicle map with {Count} entries", _deviceIdToVehicleIdMap.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error initializing device ID map");
                }
            }
        }

        private async Task StartServerAsync(CancellationToken cancellationToken)
        {
            try
            {
                _listener.Start();
                _logger.LogInformation($"TCP Server started on port {((IPEndPoint)_listener.LocalEndpoint).Port}");

                while (!cancellationToken.IsCancellationRequested)
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    Interlocked.Increment(ref _totalConnectionsReceived);
                    Interlocked.Increment(ref _activeConnections);
                    _performanceMetrics.IncrementCounter("TCP.ConnectionsAccepted");

                    _ = HandleClientAsync(client, cancellationToken);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error in TCP server");
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            // İstemci adresini benzersiz bir anahtar olarak kullan
            string clientKey = client.Client.RemoteEndPoint.ToString();

            try
            {
                using (_performanceMetrics.MeasureOperation("TcpServer.HandleClient"))
                {
                    // Retry policy ile TCP istemci işleme
                    await _retryPolicy.GetTcpRetryPolicy().ExecuteAsync(async context =>
                    {
                        using (client)
                        {
                            try
                            {
                                using var stream = client.GetStream();
                                var buffer = new byte[1024];
                                var data = new StringBuilder();

                                while (!cancellationToken.IsCancellationRequested)
                                {
                                    int bytesRead;

                                    try
                                    {
                                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogWarning(ex, "Error reading from client {ClientKey}, connection might be closed", clientKey);
                                        break;
                                    }

                                    if (bytesRead == 0) break;

                                    data.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));

                                    if (data.ToString().Contains("\n"))
                                    {
                                        var messages = data.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
                                        foreach (var message in messages)
                                        {
                                            await ProcessMessageAsync(message.Trim());
                                            Interlocked.Increment(ref _messagesProcessed);
                                            _performanceMetrics.IncrementCounter("TCP.MessagesProcessed");
                                        }
                                        data.Clear();
                                    }
                                }
                            }
                            finally
                            {
                                Interlocked.Decrement(ref _activeConnections);
                            }
                        }
                    }, new Context { ["OperationKey"] = $"HandleClient-{clientKey}" });
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Unhandled error processing client {ClientKey}", clientKey);
                Interlocked.Decrement(ref _activeConnections);
            }
        }

        private async Task ProcessMessageAsync(string message)
        {
            try
            {
                using (_performanceMetrics.MeasureOperation("TcpServer.ProcessMessage"))
                {
                    var locationData = await _dataProcessor.ProcessDataAsync(message);

                    // Batch processor'a ekle (toplu işleme kuyruğuna gönder)
                    _batchProcessor.Add(locationData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: {Message}", message);
            }
        }

        private async Task ProcessLocationBatchAsync(IEnumerable<TcpLocationData> locationUpdates)
        {
            try
            {
                using (_performanceMetrics.MeasureOperation("TcpServer.ProcessLocationBatch"))
                {
                    // DeviceId'lere göre Guid bilgilerini ön bellekten hızlıca al
                    var bulkUpdates = new List<BulkLocationUpdate>();
                    var unknownDevices = new HashSet<string>();
                    var processedVehicleIds = new List<string>();

                    await _mapLock.WaitAsync();
                    try
                    {
                        foreach (var update in locationUpdates)
                        {
                            string vehicleId = await GetVehicleIdFromDeviceIdAsync(update.DeviceId);
                            if (vehicleId != null)
                            {
                                bulkUpdates.Add(new BulkLocationUpdate
                                {
                                    VehicleId = vehicleId,
                                    Location = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                                        new GeoJson2DGeographicCoordinates(update.Longitude, update.Latitude)),
                                    Speed = update.Speed,
                                    Timestamp = DateTime.UtcNow
                                });
                                processedVehicleIds.Add(vehicleId);
                            }
                            else
                            {
                                unknownDevices.Add(update.DeviceId);
                            }
                        }
                    }
                    finally
                    {
                        _mapLock.Release();
                    }

                    // Bilinmeyen cihazlar varsa, veritabanından güncel bilgileri al
                    if (unknownDevices.Count > 0)
                    {
                        _logger.LogWarning("Found {Count} unknown devices, refreshing device map", unknownDevices.Count);
                        await InitializeDeviceMap();

                        // Yeni map ile tekrar dene
                        await _mapLock.WaitAsync();
                        try
                        {
                            foreach (var deviceId in unknownDevices)
                            {
                                string vehicleId = await GetVehicleIdFromDeviceIdAsync(deviceId);
                                if (vehicleId != null)
                                {
                                    var update = locationUpdates.First(u => u.DeviceId == deviceId);
                                    bulkUpdates.Add(new BulkLocationUpdate
                                    {
                                        VehicleId = vehicleId,
                                        Location = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                                            new GeoJson2DGeographicCoordinates(update.Longitude, update.Latitude)),
                                        Speed = update.Speed,
                                        Timestamp = DateTime.UtcNow
                                    });
                                    processedVehicleIds.Add(vehicleId);
                                }
                                else
                                {
                                    _logger.LogWarning("Device ID {DeviceId} not found even after refresh", deviceId);
                                }
                            }
                        }
                        finally
                        {
                            _mapLock.Release();
                        }
                    }

                    // Bulk update servisi ile toplu güncelleme yap
                    if (bulkUpdates.Count > 0)
                    {
                        using (_performanceMetrics.MeasureOperation("TcpServer.BulkUpdateLocations"))
                        {
                            int updateCount = await _bulkUpdateService.BulkUpdateLocationsAsync(bulkUpdates);
                            _logger.LogInformation("Processed batch of {Count} location updates", bulkUpdates.Count);
                            _performanceMetrics.IncrementCounter("DB.LocationUpdates", bulkUpdates.Count);
                        }

                        // SignalR üzerinden konumları Web istemcilerine gönder
                        if (processedVehicleIds.Count > 0)
                        {
                            using (_performanceMetrics.MeasureOperation("TcpServer.BroadcastLocations"))
                            {
                                var updatedVehicles = await _vehicleService.GetVehiclesByIdsAsync(processedVehicleIds);
                                await _locationBroadcastService.BroadcastVehicleLocationsAsync(updatedVehicles);
                                _performanceMetrics.IncrementCounter("SignalR.BroadcastedUpdates", updatedVehicles.Count());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing batch of {Count} location updates", locationUpdates.Count());
            }
        }

        private async Task<string> GetVehicleIdFromDeviceIdAsync(string deviceId)
        {
            using (_performanceMetrics.MeasureOperation("TcpServer.GetVehicleIdFromDeviceIdAsync"))
            {
                // İlk olarak ön bellekten kontrol et
                string vehicleId = null;

                await _mapLock.WaitAsync();
                try
                {
                    _deviceIdToVehicleIdMap.TryGetValue(deviceId, out vehicleId);
                }
                finally
                {
                    _mapLock.Release();
                }

                // Eğer bulunamazsa, veritabanından bul ve ön belleğe ekle
                if (vehicleId == null)
                {
                    try
                    {
                        var vehicle = await _vehicleRepository.GetVehicleByDeviceIdAsync(deviceId);
                        if (vehicle != null)
                        {
                            await _mapLock.WaitAsync();
                            try
                            {
                                _deviceIdToVehicleIdMap[deviceId] = vehicle.Id;
                                vehicleId = vehicle.Id;
                            }
                            finally
                            {
                                _mapLock.Release();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error retrieving vehicle for device ID: {DeviceId}", deviceId);
                    }
                }

                return vehicleId;
            }
        }

        private void LogConnectionMetrics(object state)
        {
            try
            {
                _logger.LogInformation(
                    "TCP Server Metrics: Total Connections: {TotalConnections}, Active Connections: {ActiveConnections}, Messages Processed: {MessagesProcessed}",
                    _totalConnectionsReceived, _activeConnections, _messagesProcessed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging connection metrics");
            }
        }

        private async Task ProcessLocationDataAsync(TcpLocationData locationData)
        {
            using (_performanceMetrics.MeasureOperation("TcpServer.ProcessLocationDataAsync"))
            {
                try
                {
                    string vehicleId = await GetVehicleIdFromDeviceIdAsync(locationData.DeviceId);
                    
                    if (vehicleId != null)
                    {
                        // Araç konumunu güncelle
                        await _vehicleService.UpdateVehicleLocationAsync(
                            vehicleId,
                            locationData.Latitude,
                            locationData.Longitude,
                            locationData.Speed);
                            
                        _logger.LogDebug("Updated location for vehicle {VehicleId} from device {DeviceId}",
                            vehicleId, locationData.DeviceId);

                        // Veramobil Gateway'e gönder
                        if (_veraMobilGatewayService != null)
                        {
                            var location = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                                new GeoJson2DGeographicCoordinates(locationData.Longitude, locationData.Latitude));

                            // Bu işlemi asenkron olarak arka planda çalıştır
                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    bool success = await _veraMobilGatewayService.SendLocationDataToGatewayAsync(
                                        vehicleId, location, locationData.Speed, DateTime.UtcNow);

                                    if (success)
                                    {
                                        _logger.LogInformation("Successfully sent location data to VeraMobil Gateway for vehicle {VehicleId}", vehicleId);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Failed to send location data to VeraMobil Gateway for vehicle {VehicleId}", vehicleId);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error sending location data to VeraMobil Gateway for vehicle {VehicleId}", vehicleId);
                                }
                            });
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No vehicle found for device ID: {DeviceId}", locationData.DeviceId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing location data for device {DeviceId}", locationData.DeviceId);
                }
            }
        }

        public void Dispose()
        {
            _mapLock.Dispose();
            _batchProcessor.StopAsync().Wait();
            _connectionPool.Dispose();
            _metricsReportingTimer?.Dispose();
        }
    }
}