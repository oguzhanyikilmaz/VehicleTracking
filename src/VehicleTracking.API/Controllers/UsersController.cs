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
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(string id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Kullanıcı bulunamadı" });
                }
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("tenant/{tenantId}")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<ActionResult<List<UserDto>>> GetByTenantId(string tenantId)
        {
            try
            {
                var users = await _userService.GetUsersByTenantIdAsync(tenantId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<ActionResult<UserDto>> Create([FromBody] UserCreateDto userCreateDto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(userCreateDto);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut]
        public async Task<ActionResult<UserDto>> Update([FromBody] UserUpdateDto userUpdateDto)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(userUpdateDto);
                if (user == null)
                {
                    return NotFound(new { message = "Kullanıcı bulunamadı" });
                }
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Kullanıcı bulunamadı" });
                }
                
                return Ok(new { message = "Kullanıcı başarıyla silindi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<ActionResult> AssignRole([FromBody] UserRoleDto userRoleDto)
        {
            try
            {
                var result = await _userService.AssignRoleToUserAsync(userRoleDto.UserId, userRoleDto.RoleId);
                if (!result)
                {
                    return NotFound(new { message = "Kullanıcı veya rol bulunamadı" });
                }
                
                return Ok(new { message = "Rol başarıyla atandı" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost("remove-role")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<ActionResult> RemoveRole([FromBody] UserRoleDto userRoleDto)
        {
            try
            {
                var result = await _userService.RemoveRoleFromUserAsync(userRoleDto.UserId, userRoleDto.RoleId);
                if (!result)
                {
                    return NotFound(new { message = "Kullanıcı veya rol bulunamadı" });
                }
                
                return Ok(new { message = "Rol başarıyla kaldırıldı" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("{userId}/roles")]
        public async Task<ActionResult<List<RoleDto>>> GetUserRoles(string userId)
        {
            try
            {
                var roles = await _userService.GetUserRolesAsync(userId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("{userId}/permissions")]
        public async Task<ActionResult<List<PermissionDto>>> GetUserPermissions(string userId)
        {
            try
            {
                var permissions = await _userService.GetUserPermissionsAsync(userId);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
   
} 