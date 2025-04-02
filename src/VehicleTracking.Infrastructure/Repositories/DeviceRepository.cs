using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;
using VehicleTracking.Infrastructure.Data;

namespace VehicleTracking.Infrastructure.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IMongoCollection<Device> _devicesCollection;

        public DeviceRepository(MongoDbContext context)
        {
            _devicesCollection = context.Devices;
        }

        public async Task<Device> GetByIdAsync(string id)
        {
            return await _devicesCollection.Find(d => d.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Device> GetBySerialNumberAsync(string serialNumber)
        {
            return await _devicesCollection.Find(d => d.SerialNumber == serialNumber).FirstOrDefaultAsync();
        }

        public async Task<List<Device>> GetAllAsync()
        {
            return await _devicesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<List<Device>> GetActiveDevicesAsync()
        {
            return await _devicesCollection.Find(d => d.IsActive).ToListAsync();
        }

        public async Task<List<Device>> GetByTenantIdAsync(string tenantId)
        {
            return await _devicesCollection.Find(d => d.TenantId == tenantId).ToListAsync();
        }

        public async Task<List<Device>> GetByVehicleIdAsync(string vehicleId)
        {
            return await _devicesCollection.Find(d => d.VehicleId == vehicleId).ToListAsync();
        }

        public async Task<List<Device>> GetUnassignedDevicesAsync()
        {
            return await _devicesCollection.Find(d => d.VehicleId == null || d.VehicleId == string.Empty).ToListAsync();
        }

        public async Task<List<Device>> GetUnassignedDevicesByTenantIdAsync(string tenantId)
        {
            return await _devicesCollection.Find(d => 
                d.TenantId == tenantId && 
                (d.VehicleId == null || d.VehicleId == string.Empty)).ToListAsync();
        }

        public async Task<Device> AddAsync(Device device)
        {
            device.CreatedAt = DateTime.UtcNow;
            await _devicesCollection.InsertOneAsync(device);
            return device;
        }

        public async Task<Device> UpdateAsync(Device device)
        {
            device.UpdatedAt = DateTime.UtcNow;
            await _devicesCollection.ReplaceOneAsync(d => d.Id == device.Id, device);
            return device;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _devicesCollection.DeleteOneAsync(d => d.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> ExistsByIdAsync(string id)
        {
            return await _devicesCollection.Find(d => d.Id == id).AnyAsync();
        }

        public async Task<bool> ExistsBySerialNumberAsync(string serialNumber)
        {
            return await _devicesCollection.Find(d => d.SerialNumber == serialNumber).AnyAsync();
        }

        public async Task<bool> UpdateConnectionInfoAsync(string id, string ipAddress, int port)
        {
            var update = Builders<Device>.Update
                .Set(d => d.IpAddress, ipAddress)
                .Set(d => d.Port, port)
                .Set(d => d.LastConnectionTime, DateTime.UtcNow)
                .Set(d => d.UpdatedAt, DateTime.UtcNow);

            var result = await _devicesCollection.UpdateOneAsync(d => d.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateLastConnectionTimeAsync(string id)
        {
            var update = Builders<Device>.Update
                .Set(d => d.LastConnectionTime, DateTime.UtcNow)
                .Set(d => d.UpdatedAt, DateTime.UtcNow);

            var result = await _devicesCollection.UpdateOneAsync(d => d.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> AssignToVehicleAsync(string deviceId, string vehicleId)
        {
            var update = Builders<Device>.Update
                .Set(d => d.VehicleId, vehicleId)
                .Set(d => d.UpdatedAt, DateTime.UtcNow);

            var result = await _devicesCollection.UpdateOneAsync(d => d.Id == deviceId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveFromVehicleAsync(string deviceId)
        {
            var update = Builders<Device>.Update
                .Set(d => d.VehicleId, string.Empty)
                .Set(d => d.UpdatedAt, DateTime.UtcNow);

            var result = await _devicesCollection.UpdateOneAsync(d => d.Id == deviceId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> ActivateAsync(string id)
        {
            var update = Builders<Device>.Update
                .Set(d => d.IsActive, true)
                .Set(d => d.ActivationDate, DateTime.UtcNow)
                .Set(d => d.UpdatedAt, DateTime.UtcNow);

            var result = await _devicesCollection.UpdateOneAsync(d => d.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeactivateAsync(string id)
        {
            var update = Builders<Device>.Update
                .Set(d => d.IsActive, false)
                .Set(d => d.UpdatedAt, DateTime.UtcNow);

            var result = await _devicesCollection.UpdateOneAsync(d => d.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<int> GetDeviceCountByTenantIdAsync(string tenantId)
        {
            return (int)await _devicesCollection.CountDocumentsAsync(d => d.TenantId == tenantId);
        }
    }
} 