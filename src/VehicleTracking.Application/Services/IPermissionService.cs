using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;

namespace VehicleTracking.Application.Services
{
    public interface IPermissionService
    {
        Task<PermissionDto> GetPermissionByIdAsync(string id);
        Task<PermissionDto> GetPermissionByNameAsync(string name);
        Task<List<PermissionDto>> GetAllPermissionsAsync();
        Task<List<PermissionDto>> GetPermissionsByModuleAsync(string module);
        Task<PermissionDto> CreatePermissionAsync(PermissionCreateDto permissionCreateDto);
        Task<PermissionDto> UpdatePermissionAsync(PermissionUpdateDto permissionUpdateDto);
        Task<bool> DeletePermissionAsync(string id);
        Task<bool> PermissionExistsAsync(string id);
        Task<bool> PermissionNameExistsAsync(string name);
        Task<List<string>> GetModulesAsync();
    }
} 