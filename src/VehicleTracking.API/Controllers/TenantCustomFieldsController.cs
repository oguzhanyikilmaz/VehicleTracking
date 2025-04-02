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
    [Authorize(Roles = "Admin,TenantAdmin")]
    public class TenantCustomFieldsController : BaseController
    {
        private readonly ITenantCustomFieldService _customFieldService;

        public TenantCustomFieldsController(ITenantCustomFieldService customFieldService)
        {
            _customFieldService = customFieldService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var customFields = await _customFieldService.GetAllCustomFieldsAsync();
                return Ok(customFields, "Tüm özel alanlar başarıyla getirildi");
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
                    return BadRequest("Özel alan ID boş olamaz");
                }

                var customField = await _customFieldService.GetCustomFieldByIdAsync(id);
                if (customField == null)
                {
                    return NotFound("Özel alan bulunamadı");
                }

                return Ok(customField, "Özel alan başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomField([FromBody] TenantCustomFieldCreateDto createDto)
        {
            try
            {
                if (createDto == null)
                {
                    return BadRequest("Özel alan bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(createDto.TenantId))
                {
                    return BadRequest("Kiracı ID boş olamaz");
                }

                if (string.IsNullOrEmpty(createDto.FieldName))
                {
                    return BadRequest("Alan adı boş olamaz");
                }

                var customField = await _customFieldService.CreateCustomFieldAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = customField.Id }, customField, "Özel alan başarıyla oluşturuldu");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomField(string id, [FromBody] TenantCustomFieldUpdateDto updateDto)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Özel alan ID boş olamaz");
                }

                if (updateDto == null)
                {
                    return BadRequest("Özel alan bilgileri boş olamaz");
                }

                updateDto.Id = id;

                var updatedField = await _customFieldService.UpdateCustomFieldAsync(updateDto);
                if (updatedField == null)
                {
                    return NotFound("Özel alan bulunamadı");
                }

                return Ok(updatedField, "Özel alan başarıyla güncellendi");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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
        public async Task<IActionResult> DeleteCustomField(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Özel alan ID boş olamaz");
                }

                var result = await _customFieldService.DeleteCustomFieldAsync(id);
                if (!result)
                {
                    return NotFound("Özel alan bulunamadı");
                }

                return Ok("Özel alan başarıyla silindi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetByTenantId(string tenantId)
        {
            try
            {
                if (string.IsNullOrEmpty(tenantId))
                {
                    return BadRequest("Kiracı ID boş olamaz");
                }

                var customFields = await _customFieldService.GetCustomFieldsByTenantIdAsync(tenantId);
                return Ok(customFields, "Kiracıya ait özel alanlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}