using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Application.Models;
using VehicleTracking.Application.Services;
using VehicleTracking.API.Services;

namespace VehicleTracking.API.Controllers
{
    [Authorize]
    public class VehiclesController : BaseController
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILocationBroadcastService _locationBroadcastService;
        
        public VehiclesController(
            IVehicleService vehicleService,
            ILocationBroadcastService locationBroadcastService)
        {
            _vehicleService = vehicleService;
            _locationBroadcastService = locationBroadcastService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var vehicles = await _vehicleService.GetAllVehiclesAsync();
                return Ok(vehicles, "Tüm araçlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveVehicles()
        {
            try
            {
                var vehicles = await _vehicleService.GetActiveVehiclesAsync();
                return Ok(vehicles, "Aktif araçlar başarıyla getirildi");
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
                    return BadRequest("Araç ID boş olamaz");
                }

                var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
                if (vehicle == null)
                {
                    return NotFound("Araç bulunamadı");
                }
                
                return Ok(vehicle, "Araç başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("device/{deviceId}")]
        public async Task<IActionResult> GetByDeviceId(string deviceId)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceId))
                {
                    return BadRequest("Cihaz ID boş olamaz");
                }

                var vehicle = await _vehicleService.GetVehicleByDeviceIdAsync(deviceId);
                if (vehicle == null)
                {
                    return NotFound("Cihaza ait araç bulunamadı");
                }
                
                return Ok(vehicle, "Cihaza ait araç başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("near")]
        public async Task<IActionResult> GetVehiclesNearLocation(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double maxDistanceKm = 10)
        {
            try
            {
                var vehicles = await _vehicleService.GetVehiclesNearLocationAsync(latitude, longitude, maxDistanceKm);
                return Ok(vehicles, "Bölgedeki araçlar başarıyla getirildi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> Create([FromBody] VehicleDto vehicleDto)
        {
            try
            {
                if (vehicleDto == null)
                {
                    return BadRequest("Araç bilgileri boş olamaz");
                }

                var vehicle = await _vehicleService.AddVehicleAsync(vehicleDto);
                return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle, "Araç başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,TenantAdmin")]
        public async Task<IActionResult> Update(string id, [FromBody] VehicleDto vehicleDto)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Araç ID boş olamaz");
                }

                if (vehicleDto == null)
                {
                    return BadRequest("Araç bilgileri boş olamaz");
                }
                
                if (id != vehicleDto.Id)
                {
                    return BadRequest("ID'ler eşleşmiyor");
                }
                
                var vehicle = await _vehicleService.UpdateVehicleAsync(vehicleDto);
                if (vehicle == null)
                {
                    return NotFound("Araç bulunamadı");
                }
                
                return Ok(vehicle, "Araç başarıyla güncellendi");
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
                    return BadRequest("Araç ID boş olamaz");
                }
                
                var result = await _vehicleService.DeleteVehicleAsync(id);
                if (!result)
                {
                    return NotFound("Araç bulunamadı");
                }
                
                return Ok("Araç başarıyla silindi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("{id}/location")]
        public async Task<IActionResult> UpdateLocationPost(string id, [FromBody] LocationUpdateDto locationDto)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Araç ID boş olamaz");
                }
                
                if (locationDto == null)
                {
                    return BadRequest("Konum bilgisi gereklidir");
                }
                
                var success = await _vehicleService.UpdateVehicleLocationAsync(
                    id,
                    locationDto.Latitude,
                    locationDto.Longitude,
                    locationDto.Speed);
                    
                if (!success)
                {
                    return NotFound("Araç bulunamadı");
                }
                
                // Güncellenmiş aracı al
                var updatedVehicle = await _vehicleService.GetVehicleByIdAsync(id);
                if (updatedVehicle != null)
                {
                    // SignalR üzerinden güncel konumu yayınla
                    await _locationBroadcastService.BroadcastVehicleLocationAsync(updatedVehicle);
                    return Ok(updatedVehicle, "Araç konumu başarıyla güncellendi");
                }
                
                return Ok("Araç konumu başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPatch("{id}/location")]
        public async Task<IActionResult> UpdateLocation(
            string id,
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double speed)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Araç ID boş olamaz");
                }
                
                var success = await _vehicleService.UpdateVehicleLocationAsync(id, latitude, longitude, speed);
                if (!success)
                {
                    return NotFound("Araç bulunamadı");
                }
                
                // Güncellenmiş aracı al
                var updatedVehicle = await _vehicleService.GetVehicleByIdAsync(id);
                if (updatedVehicle != null)
                {
                    // SignalR üzerinden güncel konumu yayınla
                    await _locationBroadcastService.BroadcastVehicleLocationAsync(updatedVehicle);
                    return Ok(updatedVehicle, "Araç konumu başarıyla güncellendi");
                }
                
                return Ok("Araç konumu başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}