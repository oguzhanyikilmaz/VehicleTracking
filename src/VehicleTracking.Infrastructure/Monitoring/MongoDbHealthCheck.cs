using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace VehicleTracking.Infrastructure.Monitoring
{
    public class MongoDbHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;

        public MongoDbHealthCheck(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = new MongoClient(_connectionString);
                var server = client.GetDatabase("admin");
                
                // Basit bir komut ile MongoDB sunucusuna bağlanılabiliyor mu diye kontrol ediyoruz
                await server.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);
                
                return HealthCheckResult.Healthy("MongoDB bağlantısı sağlandı");
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(
                    context.Registration.FailureStatus,
                    "MongoDB bağlantısı başarısız",
                    ex);
            }
        }
    }
} 