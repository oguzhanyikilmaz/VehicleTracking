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
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users, "Tüm kullanıcılar başarıyla getirildi");
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
                    return BadRequest("Kullanıcı ID boş olamaz");
                }
                
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound("Kullanıcı bulunamadı");
                }
                
                return Ok(user, "Kullanıcı başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("tenant/{tenantId}")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> GetByTenantId(string tenantId)
        {
            try
            {
                if (string.IsNullOrEmpty(tenantId))
                {
                    return BadRequest("Kiracı ID boş olamaz");
                }
                
                var users = await _userService.GetUsersByTenantIdAsync(tenantId);
                return Ok(users, "Kiracıya ait kullanıcılar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> Create([FromBody] UserCreateDto userCreateDto)
        {
            try
            {
                if (userCreateDto == null)
                {
                    return BadRequest("Kullanıcı bilgileri boş olamaz");
                }
                
                if (string.IsNullOrEmpty(userCreateDto.Username))
                {
                    return BadRequest("Kullanıcı adı boş olamaz");
                }
                
                if (string.IsNullOrEmpty(userCreateDto.Email))
                {
                    return BadRequest("E-posta adresi boş olamaz");
                }
                
                var user = await _userService.CreateUserAsync(userCreateDto);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user, "Kullanıcı başarıyla oluşturuldu");
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
        public async Task<IActionResult> Update([FromBody] UserUpdateDto userUpdateDto)
        {
            try
            {
                if (userUpdateDto == null)
                {
                    return BadRequest("Kullanıcı bilgileri boş olamaz");
                }
                
                if (string.IsNullOrEmpty(userUpdateDto.Id))
                {
                    return BadRequest("Kullanıcı ID boş olamaz");
                }
                
                var user = await _userService.UpdateUserAsync(userUpdateDto);
                if (user == null)
                {
                    return NotFound("Kullanıcı bulunamadı");
                }
                
                return Ok(user, "Kullanıcı başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Kullanıcı ID boş olamaz");
                }
                
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound("Kullanıcı bulunamadı");
                }
                
                return Ok("Kullanıcı başarıyla silindi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> AssignRole([FromBody] UserRoleDto userRoleDto)
        {
            try
            {
                if (userRoleDto == null)
                {
                    return BadRequest("Kullanıcı-rol bilgileri boş olamaz");
                }
                
                if (string.IsNullOrEmpty(userRoleDto.UserId))
                {
                    return BadRequest("Kullanıcı ID boş olamaz");
                }
                
                if (string.IsNullOrEmpty(userRoleDto.RoleId))
                {
                    return BadRequest("Rol ID boş olamaz");
                }
                
                var result = await _userService.AssignRoleToUserAsync(userRoleDto.UserId, userRoleDto.RoleId);
                if (!result)
                {
                    return BadRequest("Kullanıcı veya rol bulunamadı");
                }
                
                return Ok("Rol başarıyla atandı");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("remove-role")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> RemoveRole([FromBody] UserRoleDto userRoleDto)
        {
            try
            {
                if (userRoleDto == null)
                {
                    return BadRequest("Kullanıcı-rol bilgileri boş olamaz");
                }
                
                if (string.IsNullOrEmpty(userRoleDto.UserId))
                {
                    return BadRequest("Kullanıcı ID boş olamaz");
                }
                
                if (string.IsNullOrEmpty(userRoleDto.RoleId))
                {
                    return BadRequest("Rol ID boş olamaz");
                }
                
                var result = await _userService.RemoveRoleFromUserAsync(userRoleDto.UserId, userRoleDto.RoleId);
                if (!result)
                {
                    return BadRequest("Kullanıcı veya rol bulunamadı");
                }
                
                return Ok("Rol başarıyla kaldırıldı");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Kullanıcı ID boş olamaz");
                }
                
                var roles = await _userService.GetUserRolesAsync(userId);
                return Ok(roles, "Kullanıcı rolleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("{userId}/permissions")]
        public async Task<IActionResult> GetUserPermissions(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Kullanıcı ID boş olamaz");
                }
                
                var permissions = await _userService.GetUserPermissionsAsync(userId);
                return Ok(permissions, "Kullanıcı izinleri başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 