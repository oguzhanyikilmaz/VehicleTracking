using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;

namespace VehicleTracking.Application.Services
{
    public interface IDeviceService
    {
        Task<DeviceDto> GetDeviceByIdAsync(string id);
        Task<DeviceDto> GetDeviceBySerialNumberAsync(string serialNumber);
        Task<DeviceDto> GetDeviceByVehicleIdAsync(string vehicleId);
        Task<List<DeviceDto>> GetAllDevicesAsync();
        Task<List<DeviceDto>> GetActiveDevicesAsync();
        Task<List<DeviceDto>> GetDevicesByTenantIdAsync(string tenantId);
        Task<DeviceDto> CreateDeviceAsync(DeviceCreateDto deviceCreateDto);
        Task<DeviceDto> UpdateDeviceAsync(DeviceUpdateDto deviceUpdateDto);
        Task<bool> DeleteDeviceAsync(string id);
        Task<DeviceDto> ActivateDeviceAsync(string id);
        Task<DeviceDto> DeactivateDeviceAsync(string id);
        Task<DeviceDto> AssignDeviceToVehicleAsync(string deviceId, string vehicleId);
        Task<DeviceDto> UnassignDeviceFromVehicleAsync(string deviceId);
        Task<DeviceDto> UpdateDeviceConnectionInfoAsync(string id, string ipAddress, int port, string communicationType);
        Task<DeviceDto> UpdateLastConnectionTimeAsync(string id);
        Task<List<DeviceDto>> GetUnassignedDevicesAsync();
        Task<bool> DeviceExistsAsync(string id);
        Task<bool> SerialNumberExistsAsync(string serialNumber);
        Task<IEnumerable<DeviceDto>> GetUnassignedDevicesByTenantIdAsync(string tenantId);
    }
} 