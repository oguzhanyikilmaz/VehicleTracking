using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Http;
using MongoDB.Driver.GeoJsonObjectModel;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;
using VehicleTracking.Infrastructure.Settings;

namespace VehicleTracking.Infrastructure.Services
{
    public class VeraMobilGatewayService : IVeraMobilGatewayService
    {
        private readonly ILogger<VeraMobilGatewayService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly VeraMobilSettings _settings;

        public VeraMobilGatewayService(
            ILogger<VeraMobilGatewayService> logger,
            IVehicleRepository vehicleRepository,
            IHttpClientFactory httpClientFactory,
            IOptions<VeraMobilSettings> settings)
        {
            _logger = logger;
            _vehicleRepository = vehicleRepository;
            _settings = settings.Value;
            
            // HTTP client factory'den client al
            _httpClient = httpClientFactory.CreateClient("VeraMobilGateway");
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
        }

        /// <summary>
        /// Araç konum bilgilerini Veramobil Gateway'e gönderir
        /// </summary>
        /// <param name="vehicleId">Araç ID</param>
        /// <param name="location">Konum bilgisi</param>
        /// <param name="speed">Hız bilgisi (km/s)</param>
        /// <param name="timestamp">Zaman damgası</param>
        /// <returns>İşlemin başarılı olup olmadığı</returns>
        public async Task<bool> SendLocationDataToGatewayAsync(
            string vehicleId, 
            GeoJsonPoint<GeoJson2DGeographicCoordinates> location, 
            double speed, 
            DateTime timestamp)
        {
            try
            {
                // Araç bilgilerini veritabanından al
                var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
                
                if (vehicle == null)
                {
                    _logger.LogWarning("Araç bulunamadı: {VehicleId}", vehicleId);
                    return false;
                }

                // Query parametrelerini oluştur
                var queryParams = CreateQueryParameters(vehicle, location, speed, timestamp);

                // URL oluştur
                string queryString = queryParams.ToString();
                string requestUrl = $"{_settings.GatewayUrl}?{queryString}";

                _logger.LogInformation("Veramobil Gateway isteği gönderiliyor: {Url}", requestUrl);

                // İsteği yap
                return await SendRequestToGatewayAsync(requestUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Veramobil Gateway'e veri gönderirken hata oluştu. VehicleId: {VehicleId}", vehicleId);
                return false;
            }
        }

        /// <summary>
        /// Verimobil Gateway'e toplu konum verisi gönderme
        /// </summary>
        public async Task SendBulkLocationDataToGatewayAsync(IEnumerable<VehicleLocationData> locationDataList)
        {
            foreach (var locationData in locationDataList)
            {
                await SendLocationDataToGatewayAsync(
                    locationData.VehicleId,
                    locationData.Location,
                    locationData.Speed,
                    locationData.Timestamp);
            }
        }

        /// <summary>
        /// Gateway'e istek göndermek için query parametrelerini oluşturur
        /// </summary>
        private System.Collections.Specialized.NameValueCollection CreateQueryParameters(
            Vehicle vehicle, 
            GeoJsonPoint<GeoJson2DGeographicCoordinates> location, 
            double speed, 
            DateTime timestamp)
        {
            // Cihaz ID'sini al, eğer yok ise aracın ID'sini kullan
            string deviceId = !string.IsNullOrEmpty(vehicle.DeviceId) 
                ? vehicle.DeviceId 
                : vehicle.Id;

            // Tarih formatını ayarla
            string formattedDate = timestamp.ToString("yyyy-MM-dd HH:mm:ss");

            // Query parametrelerini oluştur
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["CIHAZID"] = deviceId;
            query["CLIENTID"] = string.Empty; // İstemci ID varsa eklenebilir
            query["X"] = location.Coordinates.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
            query["Y"] = location.Coordinates.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
            query["TARIH"] = formattedDate;
            query["HIZ"] = ((int)speed).ToString();
            query["KONUM"] = _settings.DefaultLocationDescription;
            query["IPADRESS"] = _settings.DefaultIpAddress;
            query["YON"] = "344"; // Yön bilgisi - varsayılan değer
            query["MESAFE"] = "3545046000"; // Mesafe bilgisi - varsayılan değer
            query["ALARM"] = string.Empty;
            query["SITUATIONS"] = "LOCATION";
            query["ISI"] = "999"; // Isı bilgisi - varsayılan değer
            query["TRANSNO"] = "1";
            query["STATUS"] = "1";
            query["RAPORKODU"] = "VD";
            query["SURUCU"] = "1";
            query["KONTAK"] = vehicle.EngineRunning ? "1" : "0"; // Kontak bilgisi
            query["KEY"] = _settings.ApiKey; // API anahtarı konfigürasyondan alınır

            return query;
        }

        /// <summary>
        /// Veramobil Gateway'e HTTP isteği gönderir
        /// </summary>
        private async Task<bool> SendRequestToGatewayAsync(string requestUrl)
        {
            try
            {
                // İsteği yap
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                // Sonucu kontrol et
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Veramobil Gateway yanıtı: {Content}", content);
                    return content.Contains("Success", StringComparison.OrdinalIgnoreCase);
                }

                _logger.LogWarning("Veramobil Gateway isteği başarısız. Durum kodu: {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Veramobil Gateway'e HTTP isteği gönderirken hata oluştu");
                return false;
            }
        }
    }
} 