using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;
using VehicleTracking.Infrastructure.Data;

namespace VehicleTracking.Infrastructure.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly IMongoCollection<Tenant> _tenantsCollection;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Vehicle> _vehiclesCollection;
        private readonly IMongoCollection<Device> _devicesCollection;

        public TenantRepository(MongoDbContext context)
        {
            _tenantsCollection = context.Tenants;
            _usersCollection = context.Users;
            _vehiclesCollection = context.Vehicles;
            _devicesCollection = context.Devices;
        }

        public async Task<Tenant> GetByIdAsync(string id)
        {
            return await _tenantsCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Tenant> GetByNameAsync(string name)
        {
            return await _tenantsCollection.Find(t => t.Name == name).FirstOrDefaultAsync();
        }

        public async Task<List<Tenant>> GetAllAsync()
        {
            return await _tenantsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<List<Tenant>> GetActiveTenantsAsync()
        {
            return await _tenantsCollection.Find(t => t.IsActive).ToListAsync();
        }

        public async Task<Tenant> AddAsync(Tenant tenant)
        {
            tenant.CreatedAt = DateTime.UtcNow;
            await _tenantsCollection.InsertOneAsync(tenant);
            return tenant;
        }

        public async Task<Tenant> UpdateAsync(Tenant tenant)
        {
            tenant.UpdatedAt = DateTime.UtcNow;
            await _tenantsCollection.ReplaceOneAsync(t => t.Id == tenant.Id, tenant);
            return tenant;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _tenantsCollection.DeleteOneAsync(t => t.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> ExistsByIdAsync(string id)
        {
            return await _tenantsCollection.Find(t => t.Id == id).AnyAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _tenantsCollection.Find(t => t.Name == name).AnyAsync();
        }

        public async Task<bool> ActivateTenantAsync(string id)
        {
            var update = Builders<Tenant>.Update
                .Set(t => t.IsActive, true)
                .Set(t => t.UpdatedAt, DateTime.UtcNow);

            var result = await _tenantsCollection.UpdateOneAsync(t => t.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeactivateTenantAsync(string id)
        {
            var update = Builders<Tenant>.Update
                .Set(t => t.IsActive, false)
                .Set(t => t.UpdatedAt, DateTime.UtcNow);

            var result = await _tenantsCollection.UpdateOneAsync(t => t.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateSubscriptionAsync(string id, string subscriptionPlan, int maxUsers, int maxVehicles, int maxDevices, DateTime? subscriptionEndDate)
        {
            var update = Builders<Tenant>.Update
                .Set(t => t.SubscriptionPlan, subscriptionPlan)
                .Set(t => t.MaxUsers, maxUsers)
                .Set(t => t.MaxVehicles, maxVehicles)
                .Set(t => t.MaxDevices, maxDevices)
                .Set(t => t.SubscriptionEndDate, subscriptionEndDate)
                .Set(t => t.UpdatedAt, DateTime.UtcNow);

            var result = await _tenantsCollection.UpdateOneAsync(t => t.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<int> GetUserCountAsync(string tenantId)
        {
            return (int)await _usersCollection.CountDocumentsAsync(u => u.TenantId == tenantId);
        }

        public async Task<int> GetVehicleCountAsync(string tenantId)
        {
            return (int)await _vehiclesCollection.CountDocumentsAsync(v => v.TenantId == tenantId);
        }

        public async Task<int> GetDeviceCountAsync(string tenantId)
        {
            return (int)await _devicesCollection.CountDocumentsAsync(d => d.TenantId == tenantId);
        }
    }
} 