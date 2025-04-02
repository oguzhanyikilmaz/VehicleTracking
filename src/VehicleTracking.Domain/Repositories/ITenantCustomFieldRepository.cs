using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Domain.Repositories
{
    public interface ITenantCustomFieldRepository
    {
        Task<TenantCustomField> GetByIdAsync(string id);
        Task<List<TenantCustomField>> GetAllAsync();
        Task<List<TenantCustomField>> GetByTenantIdAsync(string tenantId);
        Task<TenantCustomField> GetByTenantIdAndFieldNameAsync(string tenantId, string fieldName);
        Task<List<TenantCustomField>> GetActiveFieldsByTenantIdAsync(string tenantId);
        Task<TenantCustomField> AddAsync(TenantCustomField tenantCustomField);
        Task<TenantCustomField> UpdateAsync(TenantCustomField tenantCustomField);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteByTenantIdAsync(string tenantId);
        Task<bool> DeleteByTenantIdAndFieldNameAsync(string tenantId, string fieldName);
        Task<bool> ExistsByTenantIdAndFieldNameAsync(string tenantId, string fieldName);
    }
} 