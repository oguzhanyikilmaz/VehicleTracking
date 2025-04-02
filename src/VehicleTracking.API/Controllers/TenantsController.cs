using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Application.Models;
using VehicleTracking.Application.Services;

namespace VehicleTracking.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TenantsController : BaseController
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var tenants = await _tenantService.GetAllTenantsAsync();
                return Ok(tenants, "Tüm kiracılar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Kiracı ID boş olamaz");
                }

                var tenant = await _tenantService.GetTenantByIdAsync(id);
                if (tenant == null)
                {
                    return NotFound("Kiracı bulunamadı");
                }

                return Ok(tenant, "Kiracı başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveOnly()
        {
            try
            {
                var tenants = await _tenantService.GetActiveTenantsAsync();
                return Ok(tenants, "Aktif kiracılar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TenantCreateDto tenantCreateDto)
        {
            try
            {
                if (tenantCreateDto == null)
                {
                    return BadRequest("Kiracı bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(tenantCreateDto.Name))
                {
                    return BadRequest("Kiracı adı boş olamaz");
                }

                if (await _tenantService.ExistsByNameAsync(tenantCreateDto.Name))
                {
                    return BadRequest($"'{tenantCreateDto.Name}' adında bir kiracı zaten mevcut");
                }

                var tenant = await _tenantService.CreateTenantAsync(tenantCreateDto);
                return CreatedAtAction(nameof(GetById), new { id = tenant.Id }, tenant, "Kiracı başarıyla oluşturuldu");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TenantUpdateDto tenantUpdateDto)
        {
            try
            {
                if (tenantUpdateDto == null)
                {
                    return BadRequest("Kiracı bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(tenantUpdateDto.Id))
                {
                    return BadRequest("Kiracı ID boş olamaz");
                }

                if (string.IsNullOrEmpty(tenantUpdateDto.Name))
                {
                    return BadRequest("Kiracı adı boş olamaz");
                }

                if (!await _tenantService.TenantExistsAsync(tenantUpdateDto.Id))
                {
                    return NotFound("Kiracı bulunamadı");
                }

                // Eğer ad değiştiyse yeni adın başka bir kiracıda olup olmadığını kontrol et
                var existingTenant = await _tenantService.GetTenantByIdAsync(tenantUpdateDto.Id);
                if (existingTenant != null &&
                    existingTenant.Name != tenantUpdateDto.Name &&
                    await _tenantService.ExistsByNameAsync(tenantUpdateDto.Name))
                {
                    return BadRequest($"'{tenantUpdateDto.Name}' adında bir kiracı zaten mevcut");
                }

                var tenant = await _tenantService.UpdateTenantAsync(tenantUpdateDto);
                return Ok(tenant, "Kiracı başarıyla güncellendi");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Kiracı ID boş olamaz");
                }

                if (!await _tenantService.TenantExistsAsync(id))
                {
                    return NotFound("Kiracı bulunamadı");
                }

                var result = await _tenantService.DeleteTenantAsync(id);
                if (!result)
                {
                    return BadRequest("Kiracı silinemedi");
                }

                return Ok("Kiracı başarıyla silindi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> Activate(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Kiracı ID boş olamaz");
                }

                var tenant = await _tenantService.ActivateTenantAsync(id);
                if (tenant == null)
                {
                    return NotFound("Kiracı bulunamadı");
                }

                return Ok(tenant, "Kiracı başarıyla aktifleştirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Kiracı ID boş olamaz");
                }

                var tenant = await _tenantService.DeactivateTenantAsync(id);
                if (tenant == null)
                {
                    return NotFound("Kiracı bulunamadı");
                }

                return Ok(tenant, "Kiracı başarıyla deaktifleştirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("exists/{name}")]
        public async Task<IActionResult> ExistsByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest("Kiracı adı boş olamaz");
                }

                var exists = await _tenantService.ExistsByNameAsync(name);
                return Ok(exists, exists ? "Kiracı mevcut" : "Kiracı mevcut değil");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("custom-fields/{tenantId}")]
        public async Task<IActionResult> GetCustomFields(string tenantId)
        {
            try
            {
                if (string.IsNullOrEmpty(tenantId))
                {
                    return BadRequest("Kiracı ID boş olamaz");
                }

                var customFields = await _tenantService.GetAllCustomFieldsAsync(tenantId);
                if (customFields == null)
                {
                    return NotFound("Kiracı bulunamadı");
                }

                return Ok(customFields, "Kiracı özel alanları başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("custom-fields/{tenantId}")]
        public async Task<IActionResult> AddCustomField(string tenantId, [FromBody] TenantCustomFieldDto customFieldDto)
        {
            try
            {
                if (string.IsNullOrEmpty(tenantId))
                {
                    return BadRequest("Kiracı ID boş olamaz");
                }

                if (customFieldDto == null)
                {
                    return BadRequest("Özel alan bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(customFieldDto.FieldName))
                {
                    return BadRequest("Alan adı boş olamaz");
                }

                var customFields = await _tenantService.SetCustomFieldAsync(tenantId, customFieldDto.FieldName, customFieldDto.FieldType, customFieldDto.FieldValue);
                if (!customFields)
                    return NotFound("Kiracı bulunamadı");

                return Ok(customFields, "Kiracı özel alanı başarıyla eklendi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("custom-fields/{tenantId}/{fieldName}")]
        public async Task<IActionResult> RemoveCustomField(string tenantId, string fieldName)
        {
            try
            {
                if (string.IsNullOrEmpty(tenantId))
                {
                    return BadRequest("Kiracı ID boş olamaz");
                }

                if (string.IsNullOrEmpty(fieldName))
                {
                    return BadRequest("Alan adı boş olamaz");
                }

                var customFields = await _tenantService.DeleteCustomFieldAsync(tenantId, fieldName);
                if (customFields == null)
                {
                    return NotFound("Kiracı bulunamadı");
                }

                return Ok(customFields, "Kiracı özel alanı başarıyla kaldırıldı");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}