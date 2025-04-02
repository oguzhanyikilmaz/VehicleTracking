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
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,TenantAdmin")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> GetAll()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetById(string id)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(id);
                if (role == null)
                {
                    return NotFound(new { message = "Rol bulunamadı" });
                }
                
                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("default")]
        public async Task<ActionResult<List<RoleDto>>> GetDefaultRoles()
        {
            try
            {
                var roles = await _roleService.GetDefaultRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<RoleDto>> Create([FromBody] RoleCreateDto roleCreateDto)
        {
            try
            {
                var role = await _roleService.CreateRoleAsync(roleCreateDto);
                return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut]
        public async Task<ActionResult<RoleDto>> Update([FromBody] RoleUpdateDto roleUpdateDto)
        {
            try
            {
                var role = await _roleService.UpdateRoleAsync(roleUpdateDto);
                if (role == null)
                {
                    return NotFound(new { message = "Rol bulunamadı" });
                }
                
                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var result = await _roleService.DeleteRoleAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Rol bulunamadı" });
                }
                
                return Ok(new { message = "Rol başarıyla silindi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost("assign-permission")]
        public async Task<ActionResult> AssignPermission([FromBody] RolePermissionDto rolePermissionDto)
        {
            try
            {
                var result = await _roleService.AddPermissionToRoleAsync(rolePermissionDto.RoleId, rolePermissionDto.PermissionId);
                if (!result)
                {
                    return NotFound(new { message = "Rol veya izin bulunamadı" });
                }
                
                return Ok(new { message = "İzin başarıyla atandı" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost("remove-permission")]
        public async Task<ActionResult> RemovePermission([FromBody] RolePermissionDto rolePermissionDto)
        {
            try
            {
                var result = await _roleService.RemovePermissionFromRoleAsync(rolePermissionDto.RoleId, rolePermissionDto.PermissionId);
                if (!result)
                {
                    return NotFound(new { message = "Rol veya izin bulunamadı" });
                }
                
                return Ok(new { message = "İzin başarıyla kaldırıldı" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("{roleId}/permissions")]
        public async Task<ActionResult<List<PermissionDto>>> GetRolePermissions(string roleId)
        {
            try
            {
                var permissions = await _roleService.GetRolePermissionsAsync(roleId);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
    
    public class RolePermissionDto
    {
        public string RoleId { get; set; } = string.Empty;
        public string PermissionId { get; set; } = string.Empty;
    }
} 