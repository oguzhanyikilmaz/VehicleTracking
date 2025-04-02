using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Domain.Repositories
{
    public interface IUserRoleRepository
    {
        Task<UserRole> GetByIdAsync(string id);
        Task<List<UserRole>> GetByUserIdAsync(string userId);
        Task<List<UserRole>> GetByRoleIdAsync(string roleId);
        Task<UserRole> AddAsync(UserRole userRole);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteByUserAndRoleIdAsync(string userId, string roleId);
        Task<bool> ExistsByUserAndRoleIdAsync(string userId, string roleId);
    }
} 