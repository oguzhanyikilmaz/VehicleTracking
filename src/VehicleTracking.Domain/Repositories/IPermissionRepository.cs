using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Domain.Repositories
{
    public interface IPermissionRepository
    {
        Task<Permission> GetByIdAsync(string id);
        Task<Permission> GetByNameAsync(string name);
        Task<List<Permission>> GetAllAsync();
        Task<List<Permission>> GetByModuleAsync(string module);
        Task<Permission> AddAsync(Permission permission);
        Task<Permission> UpdateAsync(Permission permission);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsByIdAsync(string id);
        Task<bool> ExistsByNameAsync(string name);
        Task<List<Permission>> GetPermissionsByIdsAsync(IEnumerable<string> permissionIds);
        Task<List<string>> GetAllModulesAsync();
    }
} 