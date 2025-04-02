using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Domain.Repositories
{
    public interface IRolePermissionRepository
    {
        Task<RolePermission> GetByIdAsync(string id);
        Task<List<RolePermission>> GetByRoleIdAsync(string roleId);
        Task<List<RolePermission>> GetByPermissionIdAsync(string permissionId);
        Task<RolePermission> AddAsync(RolePermission rolePermission);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteByRoleAndPermissionIdAsync(string roleId, string permissionId);
        Task<bool> ExistsByRoleAndPermissionIdAsync(string roleId, string permissionId);
    }
} 