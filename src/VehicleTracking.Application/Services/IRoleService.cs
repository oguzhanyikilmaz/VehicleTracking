using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;

namespace VehicleTracking.Application.Services
{
    public interface IRoleService
    {
        Task<RoleDto> GetRoleByIdAsync(string id);
        Task<RoleDto> GetRoleByNameAsync(string name);
        Task<List<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> CreateRoleAsync(RoleCreateDto roleCreateDto);
        Task<RoleDto> UpdateRoleAsync(RoleUpdateDto roleUpdateDto);
        Task<bool> DeleteRoleAsync(string id);
        Task<bool> AssignPermissionToRoleAsync(string roleId, string permissionId);
        Task<bool> RemovePermissionFromRoleAsync(string roleId, string permissionId);
        Task<List<PermissionDto>> GetRolePermissionsAsync(string roleId);
        Task<bool> RoleExistsAsync(string id);
        Task<bool> RoleNameExistsAsync(string name);
    }
} 