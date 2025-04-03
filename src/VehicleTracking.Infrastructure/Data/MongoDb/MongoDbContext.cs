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

        // Ana Koleksiyonlar
        public IMongoCollection<Vehicle> Vehicles => 
            _database.GetCollection<Vehicle>(_settings.VehiclesCollectionName);
        
        public IMongoCollection<LocationHistory> LocationHistory => 
            _database.GetCollection<LocationHistory>(_settings.LocationHistoryCollectionName);
        
        // Kullanıcı ve Yetki Yönetimi
        public IMongoCollection<User> Users => 
            _database.GetCollection<User>(_settings.UsersCollectionName);
        
        public IMongoCollection<Role> Roles => 
            _database.GetCollection<Role>(_settings.RolesCollectionName);
        
        public IMongoCollection<Permission> Permissions => 
            _database.GetCollection<Permission>(_settings.PermissionsCollectionName);
        
        public IMongoCollection<UserRole> UserRoles => 
            _database.GetCollection<UserRole>(_settings.UserRolesCollectionName);
        
        public IMongoCollection<RolePermission> RolePermissions => 
            _database.GetCollection<RolePermission>(_settings.RolePermissionsCollectionName);
        
        // Tenant ve Cihaz Yönetimi
        public IMongoCollection<Tenant> Tenants => 
            _database.GetCollection<Tenant>(_settings.TenantsCollectionName);
        
        public IMongoCollection<Device> Devices => 
            _database.GetCollection<Device>(_settings.DevicesCollectionName);
        
        public IMongoCollection<TenantCustomField> TenantCustomFields => 
            _database.GetCollection<TenantCustomField>(_settings.TenantCustomFieldsCollectionName);

        private void CreateIndexes()
        {
            // Vehicle için indeksler
            var indexKeysBuilder = Builders<Vehicle>.IndexKeys.Ascending(v => v.DeviceId);
            var indexOptions = new CreateIndexOptions { Unique = true };
            Vehicles.Indexes.CreateOne(new CreateIndexModel<Vehicle>(indexKeysBuilder, indexOptions));

            // Geo-spatial index for location
            var geoIndexKeysBuilder = Builders<Vehicle>.IndexKeys.Geo2DSphere(v => v.Location);
            Vehicles.Indexes.CreateOne(new CreateIndexModel<Vehicle>(geoIndexKeysBuilder));

            // Index for IsActive to optimize active vehicle queries
            var isActiveIndexKeysBuilder = Builders<Vehicle>.IndexKeys.Ascending(v => v.IsActive);
            Vehicles.Indexes.CreateOne(new CreateIndexModel<Vehicle>(isActiveIndexKeysBuilder));
            
            // User collection için indeksler
            Vehicles.Indexes.CreateOne(new CreateIndexModel<Vehicle>(
                Builders<Vehicle>.IndexKeys.Ascending(v => v.TenantId), 
                new CreateIndexOptions()));
            
            // User için username ve email indeksleri
            Users.Indexes.CreateOne(new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(u => u.Username), 
                new CreateIndexOptions { Unique = true }));
            
            Users.Indexes.CreateOne(new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(u => u.Email), 
                new CreateIndexOptions { Unique = true }));
                
            // Devices için serial number indeksi
            Devices.Indexes.CreateOne(new CreateIndexModel<Device>(
                Builders<Device>.IndexKeys.Ascending(d => d.SerialNumber), 
                new CreateIndexOptions { Unique = true }));
        }
    }
} 