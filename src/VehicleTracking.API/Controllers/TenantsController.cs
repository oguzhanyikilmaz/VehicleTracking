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
    [Authorize(Roles = "Admin")]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        
        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<TenantDto>>> GetAll()
        {
            try
            {
                var tenants = await _tenantService.GetAllTenantsAsync();
                return Ok(tenants);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("active")]
        [Authorize]
        public async Task<ActionResult<List<TenantDto>>> GetActive()
        {
            try
            {
                var tenants = await _tenantService.GetActiveTenantsAsync();
                return Ok(tenants);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<TenantDto>> GetById(string id)
        {
            try
            {
                var tenant = await _tenantService.GetTenantByIdAsync(id);
                if (tenant == null)
                {
                    return NotFound(new { message = "Kiracı bulunamadı" });
                }
                
                return Ok(tenant);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<TenantDto>> Create([FromBody] TenantCreateDto tenantCreateDto)
        {
            try
            {
                var tenant = await _tenantService.CreateTenantAsync(tenantCreateDto);
                return CreatedAtAction(nameof(GetById), new { id = tenant.Id }, tenant);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut]
        public async Task<ActionResult<TenantDto>> Update([FromBody] TenantUpdateDto tenantUpdateDto)
        {
            try
            {
                var tenant = await _tenantService.UpdateTenantAsync(tenantUpdateDto);
                return Ok(tenant);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
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
                var result = await _tenantService.DeleteTenantAsync(id);
                if (result)
                {
                    return Ok(new { message = "Kiracı başarıyla silindi" });
                }
                
                return NotFound(new { message = "Kiracı bulunamadı" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost("{id}/activate")]
        public async Task<ActionResult> Activate(string id)
        {
            try
            {
                var result = await _tenantService.ActivateTenantAsync(id);
                if (result)
                {
                    return Ok(new { message = "Kiracı başarıyla aktifleştirildi" });
                }
                
                return NotFound(new { message = "Kiracı bulunamadı" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult> Deactivate(string id)
        {
            try
            {
                var result = await _tenantService.DeactivateTenantAsync(id);
                if (result)
                {
                    return Ok(new { message = "Kiracı başarıyla deaktifleştirildi" });
                }
                
                return NotFound(new { message = "Kiracı bulunamadı" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost("{id}/subscription")]
        public async Task<ActionResult> UpdateSubscription(string id, [FromBody] TenantSubscriptionUpdateDto subscriptionUpdateDto)
        {
            try
            {
                var result = await _tenantService.UpdateSubscriptionAsync(id, subscriptionUpdateDto);
                if (result)
                {
                    return Ok(new { message = "Abonelik bilgileri başarıyla güncellendi" });
                }
                
                return NotFound(new { message = "Kiracı bulunamadı" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("{id}/usage")]
        [Authorize]
        public async Task<ActionResult<TenantUsageDto>> GetUsage(string id)
        {
            try
            {
                var usage = await _tenantService.GetTenantUsageAsync(id);
                return Ok(usage);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 