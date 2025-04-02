using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Domain.Repositories
{
    public interface ITenantRepository
    {
        Task<Tenant> GetByIdAsync(string id);
        Task<Tenant> GetByNameAsync(string name);
        Task<List<Tenant>> GetAllAsync();
        Task<List<Tenant>> GetActiveTenantsAsync();
        Task<Tenant> AddAsync(Tenant tenant);
        Task<Tenant> UpdateAsync(Tenant tenant);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsByIdAsync(string id);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ActivateTenantAsync(string id);
        Task<bool> DeactivateTenantAsync(string id);
        Task<bool> UpdateSubscriptionAsync(string id, string subscriptionPlan, int maxUsers, int maxVehicles, int maxDevices, System.DateTime? subscriptionEndDate);
        Task<int> GetUserCountAsync(string tenantId);
        Task<int> GetVehicleCountAsync(string tenantId);
        Task<int> GetDeviceCountAsync(string tenantId);
    }
} 