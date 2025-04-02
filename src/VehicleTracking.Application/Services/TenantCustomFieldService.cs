using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;

namespace VehicleTracking.Application.Services
{
    public class TenantCustomFieldService : ITenantCustomFieldService
    {
        private readonly ITenantCustomFieldRepository _tenantCustomFieldRepository;
        private readonly IMapper _mapper;

        public TenantCustomFieldService(
            ITenantCustomFieldRepository tenantCustomFieldRepository,
            IMapper mapper)
        {
            _tenantCustomFieldRepository = tenantCustomFieldRepository;
            _mapper = mapper;
        }

        public async Task<TenantCustomFieldDto> GetCustomFieldByIdAsync(string id)
        {
            var customField = await _tenantCustomFieldRepository.GetByIdAsync(id);
            return _mapper.Map<TenantCustomFieldDto>(customField);
        }

        public async Task<List<TenantCustomFieldDto>> GetAllCustomFieldsAsync()
        {
            var customFields = await _tenantCustomFieldRepository.GetAllAsync();
            return _mapper.Map<List<TenantCustomFieldDto>>(customFields);
        }

        public async Task<List<TenantCustomFieldDto>> GetCustomFieldsByTenantIdAsync(string tenantId)
        {
            var customFields = await _tenantCustomFieldRepository.GetByTenantIdAsync(tenantId);
            return _mapper.Map<List<TenantCustomFieldDto>>(customFields);
        }

        public async Task<TenantCustomFieldDto> GetCustomFieldByTenantIdAndFieldNameAsync(string tenantId, string fieldName)
        {
            var customField = await _tenantCustomFieldRepository.GetByTenantIdAndFieldNameAsync(tenantId, fieldName);
            return _mapper.Map<TenantCustomFieldDto>(customField);
        }

        public async Task<List<TenantCustomFieldDto>> GetActiveCustomFieldsByTenantIdAsync(string tenantId)
        {
            var customFields = await _tenantCustomFieldRepository.GetActiveFieldsByTenantIdAsync(tenantId);
            return _mapper.Map<List<TenantCustomFieldDto>>(customFields);
        }

        public async Task<TenantCustomFieldDto> CreateCustomFieldAsync(TenantCustomFieldCreateDto customFieldCreateDto)
        {
            // Önce aynı isimde alan var mı kontrol et
            var exists = await _tenantCustomFieldRepository.ExistsByTenantIdAndFieldNameAsync(
                customFieldCreateDto.TenantId, 
                customFieldCreateDto.FieldName);
                
            if (exists)
            {
                throw new InvalidOperationException($"Bu kiracı için '{customFieldCreateDto.FieldName}' alanı zaten mevcut");
            }

            var customField = _mapper.Map<TenantCustomField>(customFieldCreateDto);
            customField = await _tenantCustomFieldRepository.AddAsync(customField);
            return _mapper.Map<TenantCustomFieldDto>(customField);
        }

        public async Task<TenantCustomFieldDto> UpdateCustomFieldAsync(TenantCustomFieldUpdateDto customFieldUpdateDto)
        {
            var existingField = await _tenantCustomFieldRepository.GetByIdAsync(customFieldUpdateDto.Id);
            if (existingField == null)
            {
                throw new KeyNotFoundException($"ID: {customFieldUpdateDto.Id} olan özel alan bulunamadı");
            }

            // Alan adı değiştiyse, yeni adla başka bir alan olup olmadığını kontrol et
            if (existingField.FieldName != customFieldUpdateDto.FieldName)
            {
                var exists = await _tenantCustomFieldRepository.ExistsByTenantIdAndFieldNameAsync(
                    existingField.TenantId, 
                    customFieldUpdateDto.FieldName);
                    
                if (exists)
                {
                    throw new InvalidOperationException($"Bu kiracı için '{customFieldUpdateDto.FieldName}' alanı zaten mevcut");
                }
            }

            // Mevcut alanı güncelle
            existingField.FieldName = customFieldUpdateDto.FieldName;
            existingField.FieldType = customFieldUpdateDto.FieldType;
            existingField.FieldValue = customFieldUpdateDto.FieldValue;
            existingField.IsActive = customFieldUpdateDto.IsActive;
            existingField.UpdatedAt = DateTime.UtcNow;

            existingField = await _tenantCustomFieldRepository.UpdateAsync(existingField);
            return _mapper.Map<TenantCustomFieldDto>(existingField);
        }

        public async Task<bool> DeleteCustomFieldAsync(string id)
        {
            return await _tenantCustomFieldRepository.DeleteAsync(id);
        }

        public async Task<bool> DeleteCustomFieldsByTenantIdAsync(string tenantId)
        {
            return await _tenantCustomFieldRepository.DeleteByTenantIdAsync(tenantId);
        }

        public async Task<bool> DeleteCustomFieldByTenantIdAndFieldNameAsync(string tenantId, string fieldName)
        {
            return await _tenantCustomFieldRepository.DeleteByTenantIdAndFieldNameAsync(tenantId, fieldName);
        }

        public async Task<bool> CustomFieldExistsByTenantIdAndFieldNameAsync(string tenantId, string fieldName)
        {
            return await _tenantCustomFieldRepository.ExistsByTenantIdAndFieldNameAsync(tenantId, fieldName);
        }

        // Yardımcı metodlar
        public async Task<bool> SetCustomFieldValueAsync(string tenantId, string fieldName, string fieldType, string fieldValue)
        {
            // Alan zaten var mı kontrol et
            var customField = await _tenantCustomFieldRepository.GetByTenantIdAndFieldNameAsync(tenantId, fieldName);

            if (customField == null)
            {
                // Alan yoksa yeni oluştur
                customField = new TenantCustomField
                {
                    TenantId = tenantId,
                    FieldName = fieldName,
                    FieldType = fieldType,
                    FieldValue = fieldValue,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _tenantCustomFieldRepository.AddAsync(customField);
            }
            else
            {
                // Alan varsa güncelle
                customField.FieldValue = fieldValue;
                customField.UpdatedAt = DateTime.UtcNow;
                await _tenantCustomFieldRepository.UpdateAsync(customField);
            }

            return true;
        }

        public async Task<string> GetCustomFieldValueAsync(string tenantId, string fieldName)
        {
            var customField = await _tenantCustomFieldRepository.GetByTenantIdAndFieldNameAsync(tenantId, fieldName);
            return customField?.FieldValue;
        }

        public async Task<Dictionary<string, string>> GetAllCustomFieldValuesForTenantAsync(string tenantId)
        {
            var customFields = await _tenantCustomFieldRepository.GetActiveFieldsByTenantIdAsync(tenantId);
            return customFields.ToDictionary(f => f.FieldName, f => f.FieldValue);
        }

        public async Task<TenantDto> EnrichTenantWithCustomFieldsAsync(TenantDto tenant)
        {
            if (tenant == null)
                return null;

            // Kiracıya ait tüm özel alanları al
            var customFields = await _tenantCustomFieldRepository.GetActiveFieldsByTenantIdAsync(tenant.Id);
            
            // Özel alanları TenantDto'daki CustomFields sözlüğüne ekle
            tenant.CustomFields = customFields.ToDictionary(f => f.FieldName, f => f.FieldValue);
            
            return tenant;
        }
    }
} 