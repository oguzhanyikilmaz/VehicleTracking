using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Domain.Repositories
{
    public interface IDeviceRepository
    {
        Task<Device> GetByIdAsync(string id);
        Task<Device> GetBySerialNumberAsync(string serialNumber);
        Task<List<Device>> GetAllAsync();
        Task<List<Device>> GetActiveDevicesAsync();
        Task<List<Device>> GetByTenantIdAsync(string tenantId);
        Task<List<Device>> GetByVehicleIdAsync(string vehicleId);
        Task<List<Device>> GetUnassignedDevicesAsync();
        Task<List<Device>> GetUnassignedDevicesByTenantIdAsync(string tenantId);
        Task<Device> AddAsync(Device device);
        Task<Device> UpdateAsync(Device device);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsByIdAsync(string id);
        Task<bool> ExistsBySerialNumberAsync(string serialNumber);
        Task<bool> UpdateConnectionInfoAsync(string id, string ipAddress, int port);
        Task<bool> UpdateLastConnectionTimeAsync(string id);
        Task<bool> AssignToVehicleAsync(string deviceId, string vehicleId);
        Task<bool> RemoveFromVehicleAsync(string deviceId);
        Task<bool> ActivateAsync(string id);
        Task<bool> DeactivateAsync(string id);
        Task<int> GetDeviceCountByTenantIdAsync(string tenantId);
    }
} 