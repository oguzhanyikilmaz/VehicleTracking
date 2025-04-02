using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;

namespace VehicleTracking.Application.Services
{
    public interface IDeviceService
    {
        Task<DeviceDto> GetDeviceByIdAsync(string id);
        Task<DeviceDto> GetDeviceBySerialNumberAsync(string serialNumber);
        Task<List<DeviceDto>> GetAllDevicesAsync();
        Task<List<DeviceDto>> GetActiveDevicesAsync();
        Task<List<DeviceDto>> GetDevicesByTenantIdAsync(string tenantId);
        Task<DeviceDto> CreateDeviceAsync(DeviceCreateDto deviceCreateDto);
        Task<DeviceDto> UpdateDeviceAsync(DeviceUpdateDto deviceUpdateDto);
        Task<bool> DeleteDeviceAsync(string id);
        Task<bool> ActivateDeviceAsync(string id);
        Task<bool> DeactivateDeviceAsync(string id);
        Task<bool> AssignDeviceToVehicleAsync(string deviceId, string vehicleId);
        Task<bool> RemoveDeviceFromVehicleAsync(string deviceId);
        Task<bool> DeviceExistsAsync(string id);
        Task<bool> SerialNumberExistsAsync(string serialNumber);
    }
} 