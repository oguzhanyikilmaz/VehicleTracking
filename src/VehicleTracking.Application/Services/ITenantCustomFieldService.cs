using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;

namespace VehicleTracking.Application.Services
{
    public interface ITenantCustomFieldService
    {
        Task<TenantCustomFieldDto> GetCustomFieldByIdAsync(string id);
        Task<List<TenantCustomFieldDto>> GetAllCustomFieldsAsync();
        Task<List<TenantCustomFieldDto>> GetCustomFieldsByTenantIdAsync(string tenantId);
        Task<TenantCustomFieldDto> GetCustomFieldByTenantIdAndFieldNameAsync(string tenantId, string fieldName);
        Task<List<TenantCustomFieldDto>> GetActiveCustomFieldsByTenantIdAsync(string tenantId);
        Task<TenantCustomFieldDto> CreateCustomFieldAsync(TenantCustomFieldCreateDto customFieldCreateDto);
        Task<TenantCustomFieldDto> UpdateCustomFieldAsync(TenantCustomFieldUpdateDto customFieldUpdateDto);
        Task<bool> DeleteCustomFieldAsync(string id);
        Task<bool> DeleteCustomFieldsByTenantIdAsync(string tenantId);
        Task<bool> DeleteCustomFieldByTenantIdAndFieldNameAsync(string tenantId, string fieldName);
        Task<bool> CustomFieldExistsByTenantIdAndFieldNameAsync(string tenantId, string fieldName);
        
        // Yardımcı metodlar
        Task<bool> SetCustomFieldValueAsync(string tenantId, string fieldName, string fieldType, string fieldValue);
        Task<string> GetCustomFieldValueAsync(string tenantId, string fieldName);
        Task<Dictionary<string, string>> GetAllCustomFieldValuesForTenantAsync(string tenantId);
        Task<TenantDto> EnrichTenantWithCustomFieldsAsync(TenantDto tenant);
    }
} 