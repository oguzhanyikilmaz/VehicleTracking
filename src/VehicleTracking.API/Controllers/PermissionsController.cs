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
    public class PermissionsController : BaseController
    {
        private readonly IPermissionService _permissionService;
        
        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var permissions = await _permissionService.GetAllPermissionsAsync();
                return Ok(permissions, "Tüm izinler başarıyla getirildi");
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
                    return BadRequest("İzin ID boş olamaz");
                }
                
                var permission = await _permissionService.GetPermissionByIdAsync(id);
                if (permission == null)
                {
                    return NotFound("İzin bulunamadı");
                }
                
                return Ok(permission, "İzin başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PermissionCreateDto permissionCreateDto)
        {
            try
            {
                if (permissionCreateDto == null)
                {
                    return BadRequest("İzin bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(permissionCreateDto.Name))
                {
                    return BadRequest("İzin adı boş olamaz");
                }

                var permission = await _permissionService.CreatePermissionAsync(permissionCreateDto);
                return CreatedAtAction(nameof(GetById), new { id = permission.Id }, permission, "İzin başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PermissionUpdateDto permissionUpdateDto)
        {
            try
            {
                if (permissionUpdateDto == null)
                {
                    return BadRequest("İzin bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(permissionUpdateDto.Id))
                {
                    return BadRequest("İzin ID boş olamaz");
                }

                var permission = await _permissionService.UpdatePermissionAsync(permissionUpdateDto);
                if (permission == null)
                {
                    return NotFound("İzin bulunamadı");
                }
                
                return Ok(permission, "İzin başarıyla güncellendi");
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
                    return BadRequest("İzin ID boş olamaz");
                }
                
                var result = await _permissionService.DeletePermissionAsync(id);
                if (!result)
                {
                    return NotFound("İzin bulunamadı");
                }
                
                return Ok("İzin başarıyla silindi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("modules")]
        public async Task<IActionResult> GetModules()
        {
            try
            {
                var modules = await _permissionService.GetModulesAsync();
                return Ok(modules, "Modüller başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("by-module/{module}")]
        public async Task<IActionResult> GetByModule(string module)
        {
            try
            {
                if (string.IsNullOrEmpty(module))
                {
                    return BadRequest("Modül adı boş olamaz");
                }
                
                var permissions = await _permissionService.GetPermissionsByModuleAsync(module);
                return Ok(permissions, $"{module} modülüne ait izinler başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 