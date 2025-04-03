using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;
using VehicleTracking.Infrastructure.Data.MongoDb;

namespace VehicleTracking.Infrastructure.Repositories
{
    public class TenantCustomFieldRepository : ITenantCustomFieldRepository
    {
        private readonly IMongoCollection<TenantCustomField> _customFieldsCollection;

        public TenantCustomFieldRepository(MongoDbContext context)
        {
            _customFieldsCollection = context.TenantCustomFields;
        }

        public async Task<TenantCustomField> GetByIdAsync(string id)
        {
            return await _customFieldsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<TenantCustomField>> GetAllAsync()
        {
            return await _customFieldsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<List<TenantCustomField>> GetByTenantIdAsync(string tenantId)
        {
            return await _customFieldsCollection.Find(x => x.TenantId == tenantId).ToListAsync();
        }

        public async Task<TenantCustomField> GetByTenantIdAndFieldNameAsync(string tenantId, string fieldName)
        {
            return await _customFieldsCollection.Find(x => x.TenantId == tenantId && x.FieldName == fieldName).FirstOrDefaultAsync();
        }

        public async Task<List<TenantCustomField>> GetActiveFieldsByTenantIdAsync(string tenantId)
        {
            return await _customFieldsCollection.Find(x => x.TenantId == tenantId && x.IsActive).ToListAsync();
        }

        public async Task<TenantCustomField> AddAsync(TenantCustomField tenantCustomField)
        {
            tenantCustomField.CreatedAt = DateTime.UtcNow;
            await _customFieldsCollection.InsertOneAsync(tenantCustomField);
            return tenantCustomField;
        }

        public async Task<TenantCustomField> UpdateAsync(TenantCustomField tenantCustomField)
        {
            tenantCustomField.UpdatedAt = DateTime.UtcNow;
            await _customFieldsCollection.ReplaceOneAsync(x => x.Id == tenantCustomField.Id, tenantCustomField);
            return tenantCustomField;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _customFieldsCollection.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteByTenantIdAsync(string tenantId)
        {
            var result = await _customFieldsCollection.DeleteManyAsync(x => x.TenantId == tenantId);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteByTenantIdAndFieldNameAsync(string tenantId, string fieldName)
        {
            var result = await _customFieldsCollection.DeleteOneAsync(x => x.TenantId == tenantId && x.FieldName == fieldName);
            return result.DeletedCount > 0;
        }

        public async Task<bool> ExistsByTenantIdAndFieldNameAsync(string tenantId, string fieldName)
        {
            return await _customFieldsCollection.Find(x => x.TenantId == tenantId && x.FieldName == fieldName).AnyAsync();
        }
    }
} 