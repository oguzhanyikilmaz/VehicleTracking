using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;
using VehicleTracking.Infrastructure.Data;
using VehicleTracking.Infrastructure.Data.MongoDb;

namespace VehicleTracking.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly IMongoCollection<Permission> _permissionsCollection;

        public PermissionRepository(MongoDbContext context)
        {
            _permissionsCollection = context.Permissions;
        }

        public async Task<Permission> GetByIdAsync(string id)
        {
            return await _permissionsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Permission> GetByNameAsync(string name)
        {
            return await _permissionsCollection.Find(p => p.Name == name).FirstOrDefaultAsync();
        }

        public async Task<List<Permission>> GetAllAsync()
        {
            return await _permissionsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<List<Permission>> GetByModuleAsync(string module)
        {
            return await _permissionsCollection.Find(p => p.Module == module).ToListAsync();
        }

        public async Task<Permission> AddAsync(Permission permission)
        {
            permission.CreatedAt = DateTime.UtcNow;
            await _permissionsCollection.InsertOneAsync(permission);
            return permission;
        }

        public async Task<Permission> UpdateAsync(Permission permission)
        {
            permission.UpdatedAt = DateTime.UtcNow;
            await _permissionsCollection.ReplaceOneAsync(p => p.Id == permission.Id, permission);
            return permission;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _permissionsCollection.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> ExistsByIdAsync(string id)
        {
            return await _permissionsCollection.Find(p => p.Id == id).AnyAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _permissionsCollection.Find(p => p.Name == name).AnyAsync();
        }

        public async Task<List<Permission>> GetPermissionsByIdsAsync(IEnumerable<string> permissionIds)
        {
            var filter = Builders<Permission>.Filter.In(p => p.Id, permissionIds);
            return await _permissionsCollection.Find(filter).ToListAsync();
        }

        public async Task<List<string>> GetAllModulesAsync()
        {
            var modules = await _permissionsCollection.Distinct(p => p.Module, FilterDefinition<Permission>.Empty).ToListAsync();
            return modules.Where(m => !string.IsNullOrEmpty(m)).Distinct().ToList();
        }
    }
}