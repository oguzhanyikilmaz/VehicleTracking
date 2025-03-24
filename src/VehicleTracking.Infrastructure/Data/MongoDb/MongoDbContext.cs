using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Infrastructure.Data.MongoDb
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDbSettings _settings;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;
            var client = new MongoClient(_settings.ConnectionString);
            _database = client.GetDatabase(_settings.DatabaseName);

            // Ensure indexes are created
            CreateIndexes();
        }

        public IMongoCollection<Vehicle> Vehicles => 
            _database.GetCollection<Vehicle>(_settings.VehiclesCollectionName);

        public IMongoCollection<LocationHistory> LocationHistory => 
            _database.GetCollection<LocationHistory>(_settings.LocationHistoryCollectionName);

        private void CreateIndexes()
        {
            var indexKeysBuilder = Builders<Vehicle>.IndexKeys.Ascending(v => v.DeviceId);
            var indexOptions = new CreateIndexOptions { Unique = true };
            Vehicles.Indexes.CreateOne(new CreateIndexModel<Vehicle>(indexKeysBuilder, indexOptions));

            // Geo-spatial index for location
            var geoIndexKeysBuilder = Builders<Vehicle>.IndexKeys.Geo2DSphere(v => v.Location);
            Vehicles.Indexes.CreateOne(new CreateIndexModel<Vehicle>(geoIndexKeysBuilder));

            // Index for IsActive to optimize active vehicle queries
            var isActiveIndexKeysBuilder = Builders<Vehicle>.IndexKeys.Ascending(v => v.IsActive);
            Vehicles.Indexes.CreateOne(new CreateIndexModel<Vehicle>(isActiveIndexKeysBuilder));
        }
    }
} 