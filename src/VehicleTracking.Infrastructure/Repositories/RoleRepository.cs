using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;
using VehicleTracking.Infrastructure.Data;

namespace VehicleTracking.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IMongoCollection<Role> _rolesCollection;
        
        public RoleRepository(MongoDbContext context)
        {
            _rolesCollection = context.Roles;
        }
        
        public async Task<Role> GetByIdAsync(string id)
        {
            return await _rolesCollection.Find(r => r.Id == id).FirstOrDefaultAsync();
        }
        
        public async Task<Role> GetByNameAsync(string name)
        {
            return await _rolesCollection.Find(r => r.Name == name).FirstOrDefaultAsync();
        }
        
        public async Task<List<Role>> GetAllAsync()
        {
            return await _rolesCollection.Find(_ => true).ToListAsync();
        }
        
        public async Task<Role> AddAsync(Role role)
        {
            role.CreatedAt = DateTime.UtcNow;
            await _rolesCollection.InsertOneAsync(role);
            return role;
        }
        
        public async Task<Role> UpdateAsync(Role role)
        {
            role.UpdatedAt = DateTime.UtcNow;
            await _rolesCollection.ReplaceOneAsync(r => r.Id == role.Id, role);
            return role;
        }
        
        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _rolesCollection.DeleteOneAsync(r => r.Id == id);
            return result.DeletedCount > 0;
        }
        
        public async Task<bool> ExistsByIdAsync(string id)
        {
            return await _rolesCollection.Find(r => r.Id == id).AnyAsync();
        }
        
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _rolesCollection.Find(r => r.Name == name).AnyAsync();
        }
        
        public async Task<bool> AddPermissionToRoleAsync(string roleId, string permissionId)
        {
            var update = Builders<Role>.Update
                .AddToSet(r => r.PermissionIds, permissionId)
                .Set(r => r.UpdatedAt, DateTime.UtcNow);
                
            var result = await _rolesCollection.UpdateOneAsync(r => r.Id == roleId, update);
            return result.ModifiedCount > 0;
        }
        
        public async Task<bool> RemovePermissionFromRoleAsync(string roleId, string permissionId)
        {
            var update = Builders<Role>.Update
                .Pull(r => r.PermissionIds, permissionId)
                .Set(r => r.UpdatedAt, DateTime.UtcNow);
                
            var result = await _rolesCollection.UpdateOneAsync(r => r.Id == roleId, update);
            return result.ModifiedCount > 0;
        }
        
        public async Task<List<string>> GetRolePermissionIdsAsync(string roleId)
        {
            var role = await _rolesCollection.Find(r => r.Id == roleId).FirstOrDefaultAsync();
            return role?.PermissionIds ?? new List<string>();
        }
        
        public async Task<List<Role>> GetRolesByIdsAsync(IEnumerable<string> roleIds)
        {
            var filter = Builders<Role>.Filter.In(r => r.Id, roleIds);
            return await _rolesCollection.Find(filter).ToListAsync();
        }
    }
} 