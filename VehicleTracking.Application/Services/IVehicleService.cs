using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;

namespace VehicleTracking.Application.Services
{
    public interface IVehicleService
    {
        Task<VehicleDto> GetVehicleByIdAsync(Guid id);
        Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
        Task<VehicleDto> CreateVehicleAsync(VehicleDto vehicleDto);
        Task UpdateVehicleAsync(VehicleDto vehicleDto);
        Task DeleteVehicleAsync(Guid id);
        Task UpdateVehicleLocationAsync(Guid id, double latitude, double longitude, double speed);
        Task<IEnumerable<VehicleDto>> GetActiveVehiclesAsync();
        Task<IEnumerable<VehicleDto>> GetVehiclesByIdsAsync(IEnumerable<Guid> ids);
    }
} 