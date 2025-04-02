using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver.GeoJsonObjectModel;
using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Domain.Repositories
{
    public interface IVehicleRepository
    {
        Task<Vehicle> GetByIdAsync(string id);
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync();
        Task<Vehicle> GetVehicleByDeviceIdAsync(string deviceId);
        Task<IEnumerable<Vehicle>> GetVehiclesByIdsAsync(IEnumerable<string> ids);
        Task<IEnumerable<Vehicle>> GetVehiclesNearLocationAsync(double latitude, double longitude, double maxDistanceKm);
        Task<Vehicle> AddAsync(Vehicle vehicle);
        Task<Vehicle> UpdateAsync(Vehicle vehicle);
        Task<bool> UpdateLocationAsync(string id, GeoJsonPoint<GeoJson2DGeographicCoordinates> location, double speed);
        Task<bool> DeleteAsync(string id);
        
        // Tenant bazlı filtreleme metodları
        Task<Vehicle> GetByIdForTenantAsync(string id, string tenantId);
        Task<IEnumerable<Vehicle>> GetAllForTenantAsync(string tenantId);
        Task<IEnumerable<Vehicle>> GetActiveVehiclesForTenantAsync(string tenantId);
        Task<IEnumerable<Vehicle>> GetVehiclesNearLocationForTenantAsync(double latitude, double longitude, double maxDistanceKm, string tenantId);
        Task<bool> VehicleBelongsToTenantAsync(string vehicleId, string tenantId);

        Task<bool> UpdateDeviceIdAsync(string vehicleId, string deviceId);
    }
} 