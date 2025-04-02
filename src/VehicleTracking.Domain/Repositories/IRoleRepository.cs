using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role> GetByIdAsync(string id);
        Task<Role> GetByNameAsync(string name);
        Task<List<Role>> GetAllAsync();
        Task<Role> AddAsync(Role role);
        Task<Role> UpdateAsync(Role role);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsByIdAsync(string id);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> AddPermissionToRoleAsync(string roleId, string permissionId);
        Task<bool> RemovePermissionFromRoleAsync(string roleId, string permissionId);
        Task<List<string>> GetRolePermissionIdsAsync(string roleId);
        Task<List<Role>> GetRolesByIdsAsync(IEnumerable<string> roleIds);
    }
} 