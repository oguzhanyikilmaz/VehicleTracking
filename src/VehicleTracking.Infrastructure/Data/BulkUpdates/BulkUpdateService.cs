using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Infrastructure.Common;
using VehicleTracking.Infrastructure.Data.MongoDb;
using VehicleTracking.Infrastructure.Services;

namespace VehicleTracking.Infrastructure.Data.BulkUpdates
{
    public class BulkUpdateService
    {
        private readonly MongoDbContext _mongoContext;
        private readonly ILogger<BulkUpdateService> _logger;
        private readonly RetryPolicy _retryPolicy;
        private readonly IVeraMobilGatewayService _veraMobilGatewayService;

        public BulkUpdateService(
            MongoDbContext mongoContext,
            ILogger<BulkUpdateService> logger,
            RetryPolicy retryPolicy,
            IVeraMobilGatewayService veraMobilGatewayService = null) // Opsiyonel parametre
        {
            _mongoContext = mongoContext;
            _logger = logger;
            _retryPolicy = retryPolicy;
            _veraMobilGatewayService = veraMobilGatewayService;
        }

        public async Task<int> BulkUpdateLocationsAsync(IEnumerable<BulkLocationUpdate> updates)
        {
            if (!updates.Any())
                return 0;

            try
            {
                int updateCount = 0;
                await _retryPolicy.GetDatabaseRetryPolicy().ExecuteAsync(async context =>
                {
                    var bulkOps = new List<WriteModel<Vehicle>>();
                    var historyInserts = new List<LocationHistory>();
                    var veraMobilUpdates = new List<VehicleLocationData>();

                    foreach (var update in updates)
                    {
                        // Update for Vehicle collection
                        var filter = Builders<Vehicle>.Filter.Eq(v => v.Id, update.VehicleId);
                        var updateDefinition = Builders<Vehicle>.Update
                            .Set(v => v.Location, update.Location)
                            .Set(v => v.Speed, update.Speed)
                            .Set(v => v.LastUpdateTime, update.Timestamp);

                        bulkOps.Add(new UpdateOneModel<Vehicle>(filter, updateDefinition));

                        // Create history record
                        historyInserts.Add(new LocationHistory
                        {
                            VehicleId = update.VehicleId,
                            Location = update.Location,
                            Speed = update.Speed,
                            Timestamp = update.Timestamp
                        });

                        // VeraMobil Gateway'e gönderilecek verileri hazırla
                        if (_veraMobilGatewayService != null)
                        {
                            veraMobilUpdates.Add(new VehicleLocationData
                            {
                                VehicleId = update.VehicleId,
                                Location = update.Location,
                                Speed = update.Speed,
                                Timestamp = update.Timestamp
                            });
                        }
                    }

                    // Execute bulk vehicle updates
                    if (bulkOps.Any())
                    {
                        var result = await _mongoContext.Vehicles.BulkWriteAsync(bulkOps);
                        updateCount = (int)result.ModifiedCount;
                        _logger.LogInformation("Bulk updated {Count} vehicle locations. Modified: {Modified}", 
                            bulkOps.Count, result.ModifiedCount);
                    }

                    // Insert location history in bulk
                    if (historyInserts.Any())
                    {
                        await _mongoContext.LocationHistory.InsertManyAsync(historyInserts);
                        _logger.LogInformation("Inserted {Count} location history records", historyInserts.Count);
                    }

                    // Verileri VeraMobil Gateway'e gönder
                    if (_veraMobilGatewayService != null && veraMobilUpdates.Any())
                    {
                        // Bu işlem asenkron olarak arka planda gerçekleşsin
                        _ = Task.Run(async () => 
                        {
                            try
                            {
                                await _veraMobilGatewayService.SendBulkLocationDataToGatewayAsync(veraMobilUpdates);
                                _logger.LogInformation("Sent {Count} location updates to VeraMobil Gateway", veraMobilUpdates.Count);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error sending location updates to VeraMobil Gateway");
                            }
                        });
                    }
                }, new Context { ["OperationKey"] = "BulkUpdateLocations" });

                return updateCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk location update for {Count} vehicles", updates.Count());
                throw;
            }
        }

        // Bu metot veritabanı nesnelerinin varlığını kontrol eder ve gerekirse oluşturur
        public async Task EnsureDatabaseObjectsAsync()
        {
            try
            {
                await _retryPolicy.GetDatabaseRetryPolicy().ExecuteAsync(async context =>
                {
                    // MongoDbContext constructor'ında zaten gerekli koleksiyonlar ve
                    // indeksler oluşturuluyor, bu nedenle burada ek bir işlem yapmaya gerek yok
                    _logger.LogInformation("Verifying MongoDB collections and indexes");
                }, new Context { ["OperationKey"] = "EnsureDatabaseObjects" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring database objects");
                throw;
            }
        }
    }
} 