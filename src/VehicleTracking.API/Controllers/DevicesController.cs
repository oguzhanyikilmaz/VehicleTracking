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
    public class DevicesController : BaseController
    {
        private readonly IDeviceService _deviceService;
        
        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var devices = await _deviceService.GetAllDevicesAsync();
                return Ok(devices, "Tüm cihazlar başarıyla getirildi");
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
                    return BadRequest("Cihaz ID boş olamaz");
                }
                
                var device = await _deviceService.GetDeviceByIdAsync(id);
                if (device == null)
                {
                    return NotFound("Cihaz bulunamadı");
                }
                
                return Ok(device, "Cihaz başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("serial/{serialNumber}")]
        public async Task<IActionResult> GetBySerialNumber(string serialNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(serialNumber))
                {
                    return BadRequest("Seri numarası boş olamaz");
                }
                
                var device = await _deviceService.GetDeviceBySerialNumberAsync(serialNumber);
                if (device == null)
                {
                    return NotFound("Cihaz bulunamadı");
                }
                
                return Ok(device, "Cihaz başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveDevices()
        {
            try
            {
                var devices = await _deviceService.GetActiveDevicesAsync();
                return Ok(devices, "Aktif cihazlar başarıyla getirildi");
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
                
                var devices = await _deviceService.GetDevicesByTenantIdAsync(tenantId);
                return Ok(devices, "Kiracıya ait cihazlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetByVehicleId(string vehicleId)
        {
            try
            {
                if (string.IsNullOrEmpty(vehicleId))
                {
                    return BadRequest("Araç ID boş olamaz");
                }
                
                var device = await _deviceService.GetDeviceByVehicleIdAsync(vehicleId);
                if (device == null)
                {
                    return NotFound("Araca atanmış cihaz bulunamadı");
                }
                
                return Ok(device, "Araca atanmış cihaz başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("unassigned")]
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
        
        [HttpPost]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> Create([FromBody] DeviceCreateDto deviceCreateDto)
        {
            try
            {
                if (deviceCreateDto == null)
                {
                    return BadRequest("Cihaz bilgileri boş olamaz");
                }
                
                if (string.IsNullOrEmpty(deviceCreateDto.SerialNumber))
                {
                    return BadRequest("Seri numarası boş olamaz");
                }
                
                if (string.IsNullOrEmpty(deviceCreateDto.Model))
                {
                    return BadRequest("Model boş olamaz");
                }
                
                var device = await _deviceService.CreateDeviceAsync(deviceCreateDto);
                return CreatedAtAction(nameof(GetById), new { id = device.Id }, device, "Cihaz başarıyla oluşturuldu");
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
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> Update([FromBody] DeviceUpdateDto deviceUpdateDto)
        {
            try
            {
                if (deviceUpdateDto == null)
                {
                    return BadRequest("Cihaz bilgileri boş olamaz");
                }
                
                if (string.IsNullOrEmpty(deviceUpdateDto.Id))
                {
                    return BadRequest("Cihaz ID boş olamaz");
                }
                
                var device = await _deviceService.UpdateDeviceAsync(deviceUpdateDto);
                if (device == null)
                {
                    return NotFound("Cihaz bulunamadı");
                }
                
                return Ok(device, "Cihaz başarıyla güncellendi");
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
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Cihaz ID boş olamaz");
                }
                
                var result = await _deviceService.DeleteDeviceAsync(id);
                if (!result)
                {
                    return NotFound("Cihaz bulunamadı");
                }
                
                return Ok("Cihaz başarıyla silindi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> ActivateDevice(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Cihaz ID boş olamaz");
                }
                
                var device = await _deviceService.ActivateDeviceAsync(id);
                return Ok(device, "Cihaz başarıyla aktifleştirildi");
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
        
        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> DeactivateDevice(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Cihaz ID boş olamaz");
                }
                
                var device = await _deviceService.DeactivateDeviceAsync(id);
                return Ok(device, "Cihaz başarıyla deaktifleştirildi");
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
        
        [HttpPost("{id}/assign-to-vehicle/{vehicleId}")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> AssignToVehicle(string id, string vehicleId)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Cihaz ID boş olamaz");
                }
                
                if (string.IsNullOrEmpty(vehicleId))
                {
                    return BadRequest("Araç ID boş olamaz");
                }
                
                var device = await _deviceService.AssignDeviceToVehicleAsync(id, vehicleId);
                return Ok(device, "Cihaz araca başarıyla atandı");
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
        
        [HttpPost("{id}/unassign-from-vehicle")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> UnassignFromVehicle(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Cihaz ID boş olamaz");
                }
                
                var device = await _deviceService.UnassignDeviceFromVehicleAsync(id);
                return Ok(device, "Cihaz araçtan başarıyla ayrıldı");
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
        
        [HttpPost("{id}/update-connection-info")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> UpdateConnectionInfo(string id, [FromBody] DeviceConnectionInfoDto connectionInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Cihaz ID boş olamaz");
                }
                
                if (connectionInfo == null)
                {
                    return BadRequest("Bağlantı bilgileri boş olamaz");
                }
                
                var device = await _deviceService.UpdateDeviceConnectionInfoAsync(id, connectionInfo.IpAddress, connectionInfo.Port, connectionInfo.CommunicationType);
                return Ok(device, "Cihaz bağlantı bilgileri başarıyla güncellendi");
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
        
        [HttpPost("{id}/update-last-connection-time")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> UpdateLastConnectionTime(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Cihaz ID boş olamaz");
                }
                
                var device = await _deviceService.UpdateLastConnectionTimeAsync(id);
                return Ok(device, "Cihaz son bağlantı zamanı başarıyla güncellendi");
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
    }
} 