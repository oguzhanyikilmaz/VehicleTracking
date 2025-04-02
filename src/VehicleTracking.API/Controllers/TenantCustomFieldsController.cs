using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Application.Services;

namespace VehicleTracking.API.Controllers
{
    [ApiController]
    [Route("api/tenants/{tenantId}/customfields")]
    [Authorize]
    public class TenantCustomFieldsController : ControllerBase
    {
        private readonly ITenantCustomFieldService _tenantCustomFieldService;
        private readonly ITenantService _tenantService;

        public TenantCustomFieldsController(
            ITenantCustomFieldService tenantCustomFieldService,
            ITenantService tenantService)
        {
            _tenantCustomFieldService = tenantCustomFieldService;
            _tenantService = tenantService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TenantCustomFieldDto>>> GetCustomFields(string tenantId)
        {
            // Kiracı var mı kontrol et
            if (!await _tenantService.TenantExistsAsync(tenantId))
            {
                return NotFound($"Kiracı bulunamadı (ID: {tenantId})");
            }

            var customFields = await _tenantCustomFieldService.GetCustomFieldsByTenantIdAsync(tenantId);
            return Ok(customFields);
        }

        [HttpGet("{fieldName}")]
        public async Task<ActionResult<TenantCustomFieldDto>> GetCustomField(string tenantId, string fieldName)
        {
            // Kiracı var mı kontrol et
            if (!await _tenantService.TenantExistsAsync(tenantId))
            {
                return NotFound($"Kiracı bulunamadı (ID: {tenantId})");
            }

            var customField = await _tenantCustomFieldService.GetCustomFieldByTenantIdAndFieldNameAsync(tenantId, fieldName);
            if (customField == null)
            {
                return NotFound($"Özel alan bulunamadı: {fieldName}");
            }

            return Ok(customField);
        }

        [HttpPost]
        public async Task<ActionResult<TenantCustomFieldDto>> CreateCustomField(string tenantId, [FromBody] TenantCustomFieldCreateDto createDto)
        {
            try
            {
                // Kiracı var mı kontrol et
                if (!await _tenantService.TenantExistsAsync(tenantId))
                {
                    return NotFound($"Kiracı bulunamadı (ID: {tenantId})");
                }

                // Tenant ID'yi yol parametresinden al
                createDto.TenantId = tenantId;

                var customField = await _tenantCustomFieldService.CreateCustomFieldAsync(createDto);
                return CreatedAtAction(nameof(GetCustomField), 
                    new { tenantId = tenantId, fieldName = customField.FieldName }, 
                    customField);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{fieldName}")]
        public async Task<ActionResult<TenantCustomFieldDto>> UpdateCustomField(
            string tenantId, 
            string fieldName, 
            [FromBody] TenantCustomFieldUpdateDto updateDto)
        {
            try
            {
                // Kiracı var mı kontrol et
                if (!await _tenantService.TenantExistsAsync(tenantId))
                {
                    return NotFound($"Kiracı bulunamadı (ID: {tenantId})");
                }

                // Özel alan var mı kontrol et
                var existingField = await _tenantCustomFieldService.GetCustomFieldByTenantIdAndFieldNameAsync(tenantId, fieldName);
                if (existingField == null)
                {
                    return NotFound($"Özel alan bulunamadı: {fieldName}");
                }

                // Güncelleme için ID ve FieldName parametrelerini ayarla
                updateDto.Id = existingField.Id;
                if (string.IsNullOrEmpty(updateDto.FieldName))
                {
                    updateDto.FieldName = fieldName;
                }

                var updatedField = await _tenantCustomFieldService.UpdateCustomFieldAsync(updateDto);
                return Ok(updatedField);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{fieldName}")]
        public async Task<ActionResult> DeleteCustomField(string tenantId, string fieldName)
        {
            // Kiracı var mı kontrol et
            if (!await _tenantService.TenantExistsAsync(tenantId))
            {
                return NotFound($"Kiracı bulunamadı (ID: {tenantId})");
            }

            var result = await _tenantCustomFieldService.DeleteCustomFieldByTenantIdAndFieldNameAsync(tenantId, fieldName);
            if (!result)
            {
                return NotFound($"Özel alan bulunamadı: {fieldName}");
            }

            return NoContent();
        }

        [HttpGet("values")]
        public async Task<ActionResult<Dictionary<string, string>>> GetAllCustomFieldValues(string tenantId)
        {
            // Kiracı var mı kontrol et
            if (!await _tenantService.TenantExistsAsync(tenantId))
            {
                return NotFound($"Kiracı bulunamadı (ID: {tenantId})");
            }

            var values = await _tenantService.GetAllCustomFieldsAsync(tenantId);
            return Ok(values);
        }

        [HttpGet("values/{fieldName}")]
        public async Task<ActionResult<string>> GetCustomFieldValue(string tenantId, string fieldName)
        {
            try
            {
                // Kiracı var mı kontrol et
                if (!await _tenantService.TenantExistsAsync(tenantId))
                {
                    return NotFound($"Kiracı bulunamadı (ID: {tenantId})");
                }

                var value = await _tenantService.GetCustomFieldValueAsync(tenantId, fieldName);
                if (value == null)
                {
                    return NotFound($"Özel alan bulunamadı: {fieldName}");
                }

                return Ok(value);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("values")]
        public async Task<ActionResult> SetCustomFieldValue(
            string tenantId, 
            [FromBody] TenantCustomFieldValueDto valueDto)
        {
            try
            {
                // Kiracı var mı kontrol et
                if (!await _tenantService.TenantExistsAsync(tenantId))
                {
                    return NotFound($"Kiracı bulunamadı (ID: {tenantId})");
                }

                // Alan tipini otomatik olarak string olarak ayarla (basitleştirme için)
                // Gerçek uygulamada, değerin tipine göre doğru tip belirlenmeli
                string fieldType = "String";

                await _tenantService.SetCustomFieldAsync(tenantId, valueDto.FieldName, fieldType, valueDto.FieldValue);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 