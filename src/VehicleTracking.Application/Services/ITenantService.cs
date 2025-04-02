using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;

namespace VehicleTracking.Application.Services
{
    public interface ITenantService
    {
        Task<TenantDto> GetTenantByIdAsync(string id);
        Task<List<TenantDto>> GetAllTenantsAsync();
        Task<List<TenantDto>> GetActiveTenantsAsync();
        Task<TenantDto> CreateTenantAsync(TenantCreateDto tenantCreateDto);
        Task<TenantDto> UpdateTenantAsync(TenantUpdateDto tenantUpdateDto);
        Task<bool> DeleteTenantAsync(string id);
        Task<bool> ActivateTenantAsync(string id);
        Task<bool> DeactivateTenantAsync(string id);
        Task<bool> UpdateSubscriptionAsync(string id, TenantSubscriptionUpdateDto subscriptionUpdateDto);
        Task<bool> TenantExistsAsync(string id);
        Task<bool> ExistsByNameAsync(string name);
        Task<TenantUsageDto> GetTenantUsageAsync(string id);
        
        // Özel alan yönetimi için ek metodlar
        Task<bool> SetCustomFieldAsync(string tenantId, string fieldName, string fieldType, string fieldValue);
        Task<string> GetCustomFieldValueAsync(string tenantId, string fieldName);
        Task<Dictionary<string, string>> GetAllCustomFieldsAsync(string tenantId);
    }
} 