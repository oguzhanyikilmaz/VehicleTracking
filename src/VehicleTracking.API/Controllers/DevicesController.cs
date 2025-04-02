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
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetAllDevices()
        {
            var devices = await _deviceService.GetAllDevicesAsync();
            return Ok(devices);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetActiveDevices()
        {
            var devices = await _deviceService.GetActiveDevicesAsync();
            return Ok(devices);
        }

        [HttpGet("unassigned")]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetUnassignedDevices()
        {
            var devices = await _deviceService.GetUnassignedDevicesAsync();
            return Ok(devices);
        }

        [HttpGet("tenant/{tenantId}")]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetDevicesByTenant(string tenantId)
        {
            var devices = await _deviceService.GetDevicesByTenantIdAsync(tenantId);
            return Ok(devices);
        }

        [HttpGet("tenant/{tenantId}/unassigned")]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> GetUnassignedDevicesByTenant(string tenantId)
        {
            var devices = await _deviceService.GetUnassignedDevicesByTenantIdAsync(tenantId);
            return Ok(devices);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceDto>> GetDeviceById(string id)
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            return Ok(device);
        }

        [HttpGet("serial/{serialNumber}")]
        public async Task<ActionResult<DeviceDto>> GetDeviceBySerialNumber(string serialNumber)
        {
            var device = await _deviceService.GetDeviceBySerialNumberAsync(serialNumber);
            if (device == null)
            {
                return NotFound();
            }

            return Ok(device);
        }

        [HttpPost]
        public async Task<ActionResult<DeviceDto>> CreateDevice([FromBody] DeviceCreateDto deviceCreateDto)
        {
            try
            {
                var device = await _deviceService.CreateDeviceAsync(deviceCreateDto);
                return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, device);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DeviceDto>> UpdateDevice(string id, [FromBody] DeviceUpdateDto deviceUpdateDto)
        {
            try
            {
                if (id != deviceUpdateDto.Id)
                {
                    return BadRequest("ID'ler eşleşmiyor");
                }

                var device = await _deviceService.UpdateDeviceAsync(deviceUpdateDto);
                return Ok(device);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDevice(string id)
        {
            var result = await _deviceService.DeleteDeviceAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("{id}/activate")]
        public async Task<ActionResult> ActivateDevice(string id)
        {
            try
            {
                var result = await _deviceService.ActivateDeviceAsync(id);
                if (result)
                {
                    return NoContent();
                }
                return BadRequest("Cihaz aktifleştirilemedi");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult> DeactivateDevice(string id)
        {
            try
            {
                var result = await _deviceService.DeactivateDeviceAsync(id);
                if (result)
                {
                    return NoContent();
                }
                return BadRequest("Cihaz deaktif edilemedi");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{deviceId}/assign/{vehicleId}")]
        public async Task<ActionResult> AssignDeviceToVehicle(string deviceId, string vehicleId)
        {
            try
            {
                var result = await _deviceService.AssignDeviceToVehicleAsync(deviceId, vehicleId);
                if (result)
                {
                    return NoContent();
                }
                return BadRequest("Cihaz araca atanamadı");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{deviceId}/unassign")]
        public async Task<ActionResult> RemoveDeviceFromVehicle(string deviceId)
        {
            try
            {
                var result = await _deviceService.RemoveDeviceFromVehicleAsync(deviceId);
                if (result)
                {
                    return NoContent();
                }
                return BadRequest("Cihaz araçtan kaldırılamadı");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id}/connection")]
        public async Task<ActionResult> UpdateConnectionInfo(string id, [FromBody] DeviceConnectionUpdateDto connectionUpdateDto)
        {
            try
            {
                if (id != connectionUpdateDto.DeviceId)
                {
                    return BadRequest("ID'ler eşleşmiyor");
                }

                var result = await _deviceService.UpdateConnectionInfoAsync(
                    id, 
                    connectionUpdateDto.IpAddress, 
                    connectionUpdateDto.Port);

                if (result)
                {
                    return NoContent();
                }
                
                return BadRequest("Bağlantı bilgileri güncellenemedi");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id}/heartbeat")]
        public async Task<ActionResult> UpdateLastConnectionTime(string id)
        {
            try
            {
                var result = await _deviceService.UpdateLastConnectionTimeAsync(id);
                if (result)
                {
                    return NoContent();
                }
                
                return BadRequest("Son bağlantı zamanı güncellenemedi");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
} 