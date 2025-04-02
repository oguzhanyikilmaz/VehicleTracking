using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Infrastructure.Settings;

namespace VehicleTracking.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        // Koleksiyonlar
        public IMongoCollection<Vehicle> Vehicles => _database.GetCollection<Vehicle>("Vehicles");
        public IMongoCollection<LocationHistory> LocationHistory => _database.GetCollection<LocationHistory>("LocationHistory");
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Role> Roles => _database.GetCollection<Role>("Roles");
        public IMongoCollection<Permission> Permissions => _database.GetCollection<Permission>("Permissions");
        public IMongoCollection<Tenant> Tenants => _database.GetCollection<Tenant>("Tenants");
        public IMongoCollection<Device> Devices => _database.GetCollection<Device>("Devices");
        public IMongoCollection<TenantCustomField> TenantCustomFields => _database.GetCollection<TenantCustomField>("TenantCustomFields");
    }
} 