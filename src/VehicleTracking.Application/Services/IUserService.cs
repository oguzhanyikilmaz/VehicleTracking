using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;

namespace VehicleTracking.Application.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(string id);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<List<UserDto>> GetUsersByTenantIdAsync(string tenantId);
        Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto);
        Task<UserDto> UpdateUserAsync(UserUpdateDto userUpdateDto);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<bool> AssignRoleToUserAsync(string userId, string roleId);
        Task<bool> RemoveRoleFromUserAsync(string userId, string roleId);
        Task<List<RoleDto>> GetUserRolesAsync(string userId);
        Task<List<PermissionDto>> GetUserPermissionsAsync(string userId);
        Task<bool> UserExistsAsync(string id);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
} 