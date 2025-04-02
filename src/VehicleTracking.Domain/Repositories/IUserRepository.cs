using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(string id);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync();
        Task<List<User>> GetByTenantIdAsync(string tenantId);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsByIdAsync(string id);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> UpdateRefreshTokenAsync(string id, string refreshToken, System.DateTime refreshTokenExpiryTime);
        Task<bool> UpdateLastLoginTimeAsync(string id);
        Task<User> GetByRefreshTokenAsync(string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string id);
        Task<bool> ConfirmEmailAsync(string id);
        Task<bool> AddToRoleAsync(string userId, string roleId);
        Task<bool> RemoveFromRoleAsync(string userId, string roleId);
        Task<List<string>> GetUserRoleIdsAsync(string userId);
        Task<bool> UpdatePasswordAsync(string id, string passwordHash);
    }
} 