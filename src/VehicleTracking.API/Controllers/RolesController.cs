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
    public class RolesController : BaseController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                return Ok(roles, "Tüm roller başarıyla getirildi");
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
                    return BadRequest("Rol ID boş olamaz");
                }
                
                var role = await _roleService.GetRoleByIdAsync(id);
                if (role == null)
                {
                    return NotFound("Rol bulunamadı");
                }

                return Ok(role, "Rol başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("default")]
        public async Task<IActionResult> GetDefaultRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                return Ok(roles, "Varsayılan roller başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleCreateDto roleCreateDto)
        {
            try
            {
                if (roleCreateDto == null)
                {
                    return BadRequest("Rol bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(roleCreateDto.Name))
                {
                    return BadRequest("Rol adı boş olamaz");
                }

                if (await _roleService.RoleNameExistsAsync(roleCreateDto.Name))
                {
                    return BadRequest($"'{roleCreateDto.Name}' adında bir rol zaten mevcut");
                }

                var role = await _roleService.CreateRoleAsync(roleCreateDto);
                return CreatedAtAction(nameof(GetById), new { id = role.Id }, role, "Rol başarıyla oluşturuldu");
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
        public async Task<IActionResult> Update([FromBody] RoleUpdateDto roleUpdateDto)
        {
            try
            {
                if (roleUpdateDto == null)
                {
                    return BadRequest("Rol bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(roleUpdateDto.Id))
                {
                    return BadRequest("Rol ID boş olamaz");
                }

                if (string.IsNullOrEmpty(roleUpdateDto.Name))
                {
                    return BadRequest("Rol adı boş olamaz");
                }

                if (!await _roleService.RoleExistsAsync(roleUpdateDto.Id))
                {
                    return NotFound("Rol bulunamadı");
                }

                // Rol adı var mı ve aynı ID'ye sahip değil mi kontrol et
                var existingByName = await _roleService.GetRoleByNameAsync(roleUpdateDto.Name);
                if (existingByName != null && existingByName.Id != roleUpdateDto.Id)
                {
                    return BadRequest($"'{roleUpdateDto.Name}' adında bir rol zaten mevcut");
                }

                var role = await _roleService.UpdateRoleAsync(roleUpdateDto);
                return Ok(role, "Rol başarıyla güncellendi");
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
                    return BadRequest("Rol ID boş olamaz");
                }

                if (!await _roleService.RoleExistsAsync(id))
                {
                    return NotFound("Rol bulunamadı");
                }

                var result = await _roleService.DeleteRoleAsync(id);
                if (!result)
                {
                    return BadRequest("Rol silinemedi");
                }

                return Ok("Rol başarıyla silindi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("assign-permission")]
        public async Task<IActionResult> AssignPermission([FromBody] RolePermissionDto rolePermissionDto)
        {
            try
            {
                if (rolePermissionDto == null)
                {
                    return BadRequest("Rol-izin bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(rolePermissionDto.RoleId))
                {
                    return BadRequest("Rol ID boş olamaz");
                }

                if (string.IsNullOrEmpty(rolePermissionDto.PermissionId))
                {
                    return BadRequest("İzin ID boş olamaz");
                }

                // Rol var mı kontrol et
                if (!await _roleService.RoleExistsAsync(rolePermissionDto.RoleId))
                {
                    return NotFound("Rol bulunamadı");
                }

                var result = await _roleService.AssignPermissionToRoleAsync(rolePermissionDto.RoleId, rolePermissionDto.PermissionId);
                if (!result)
                {
                    return BadRequest("İzin rol ile ilişkilendirilemedi");
                }

                return Ok("İzin başarıyla atandı");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("remove-permission")]
        public async Task<IActionResult> RemovePermission([FromBody] RolePermissionDto rolePermissionDto)
        {
            try
            {
                if (rolePermissionDto == null)
                {
                    return BadRequest("Rol-izin bilgileri boş olamaz");
                }

                if (string.IsNullOrEmpty(rolePermissionDto.RoleId))
                {
                    return BadRequest("Rol ID boş olamaz");
                }

                if (string.IsNullOrEmpty(rolePermissionDto.PermissionId))
                {
                    return BadRequest("İzin ID boş olamaz");
                }

                // Rol var mı kontrol et
                if (!await _roleService.RoleExistsAsync(rolePermissionDto.RoleId))
                {
                    return NotFound("Rol bulunamadı");
                }

                var result = await _roleService.RemovePermissionFromRoleAsync(rolePermissionDto.RoleId, rolePermissionDto.PermissionId);
                if (!result)
                {
                    return BadRequest("İzin rolden kaldırılamadı");
                }

                return Ok("İzin başarıyla kaldırıldı");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{roleId}/permissions")]
        public async Task<IActionResult> GetRolePermissions(string roleId)
        {
            try
            {
                if (string.IsNullOrEmpty(roleId))
                {
                    return BadRequest("Rol ID boş olamaz");
                }

                if (!await _roleService.RoleExistsAsync(roleId))
                {
                    return NotFound("Rol bulunamadı");
                }

                var permissions = await _roleService.GetRolePermissionsAsync(roleId);
                return Ok(permissions, "Rol izinleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}