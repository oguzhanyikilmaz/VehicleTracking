using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Domain.Repositories
{
    public interface IVehicleRepository
    {
        Task<Vehicle> GetByIdAsync(Guid id);
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<Vehicle> AddAsync(Vehicle vehicle);
        Task UpdateAsync(Vehicle vehicle);
        Task DeleteAsync(Guid id);
        Task UpdateLocationAsync(Guid id, double latitude, double longitude, double speed);
        Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync();
        Task<IEnumerable<Vehicle>> GetVehiclesByIdsAsync(IEnumerable<Guid> ids);
    }
} 