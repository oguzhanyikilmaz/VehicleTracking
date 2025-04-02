using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleTracking.Application.Models;
using VehicleTracking.Application.Services;

namespace VehicleTracking.API.Controllers
{
    [Authorize(Roles = "Admin,TenantAdmin")]
    public class UnassignedDevicesController : BaseController
    {
        private readonly IDeviceService _deviceService;
        
        public UnassignedDevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUnassignedDevices()
        {
            try
            {
                var devices = await _deviceService.GetUnassignedDevicesAsync();
                return Ok(devices, "Atanmamış cihazlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetUnassignedDevicesByTenant(string tenantId)
        {
            try
            {
                if (string.IsNullOrEmpty(tenantId))
                {
                    return BadRequest("Kiracı ID boş olamaz");
                }
                
                var devices = await _deviceService.GetUnassignedDevicesByTenantIdAsync(tenantId);
                return Ok(devices, "Kiracıya ait atanmamış cihazlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 