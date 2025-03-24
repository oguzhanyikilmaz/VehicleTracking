using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;

namespace VehicleTracking.Application.Services
{
    public interface IVehicleService
    {
        Task<VehicleDto> GetVehicleByIdAsync(string id);
        Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
        Task<IEnumerable<VehicleDto>> GetActiveVehiclesAsync();
        Task<VehicleDto> GetVehicleByDeviceIdAsync(string deviceId);
        Task<IEnumerable<VehicleDto>> GetVehiclesByIdsAsync(IEnumerable<string> ids);
        Task<IEnumerable<VehicleDto>> GetVehiclesNearLocationAsync(double latitude, double longitude, double maxDistanceKm);
        Task<VehicleDto> AddVehicleAsync(VehicleDto vehicleDto);
        Task<VehicleDto> UpdateVehicleAsync(VehicleDto vehicleDto);
        Task<bool> UpdateVehicleLocationAsync(string id, double latitude, double longitude, double speed);
        Task<bool> DeleteVehicleAsync(string id);
    }
} 