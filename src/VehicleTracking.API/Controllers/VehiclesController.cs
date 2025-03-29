using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Application.Services;
using VehicleTracking.API.Services;

namespace VehicleTracking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
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
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAllVehicles()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetActiveVehicles()
        {
            var vehicles = await _vehicleService.GetActiveVehiclesAsync();
            return Ok(vehicles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleDto>> GetVehicle(string id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            return Ok(vehicle);
        }

        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<VehicleDto>> GetVehicleByDeviceId(string deviceId)
        {
            var vehicle = await _vehicleService.GetVehicleByDeviceIdAsync(deviceId);
            if (vehicle == null)
                return NotFound();

            return Ok(vehicle);
        }

        [HttpGet("near")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetVehiclesNearLocation(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double maxDistanceKm = 10)
        {
            var vehicles = await _vehicleService.GetVehiclesNearLocationAsync(
                latitude, longitude, maxDistanceKm);
            return Ok(vehicles);
        }

        [HttpPost]
        public async Task<ActionResult<VehicleDto>> CreateVehicle(VehicleDto vehicleDto)
        {
            var result = await _vehicleService.AddVehicleAsync(vehicleDto);
            return CreatedAtAction(nameof(GetVehicle), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(string id, VehicleDto vehicleDto)
        {
            if (id != vehicleDto.Id)
                return BadRequest();

            var updated = await _vehicleService.UpdateVehicleAsync(vehicleDto);
            if (updated == null)
                return NotFound();
                
            return NoContent();
        }

        [HttpPost("{id}/location")]
        public async Task<IActionResult> UpdateLocationPost(string id, [FromBody] LocationUpdateDto locationDto)
        {
            if (locationDto == null)
                return BadRequest("Konum bilgisi gereklidir.");
                
            var success = await _vehicleService.UpdateVehicleLocationAsync(
                id, 
                locationDto.Latitude, 
                locationDto.Longitude, 
                locationDto.Speed);
                
            if (!success)
                return NotFound();
                
            // Güncellenmiş aracı al
            var updatedVehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (updatedVehicle != null)
            {
                // SignalR üzerinden güncel konumu yayınla
                await _locationBroadcastService.BroadcastVehicleLocationAsync(updatedVehicle);
            }
                
            return NoContent();
        }

        [HttpPatch("{id}/location")]
        public async Task<IActionResult> UpdateLocation(
            string id, 
            [FromQuery] double latitude, 
            [FromQuery] double longitude, 
            [FromQuery] double speed)
        {
            var success = await _vehicleService.UpdateVehicleLocationAsync(id, latitude, longitude, speed);
            if (!success)
                return NotFound();
                
            // Güncellenmiş aracı al
            var updatedVehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (updatedVehicle != null)
            {
                // SignalR üzerinden güncel konumu yayınla
                await _locationBroadcastService.BroadcastVehicleLocationAsync(updatedVehicle);
            }
                
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(string id)
        {
            var success = await _vehicleService.DeleteVehicleAsync(id);
            if (!success)
                return NotFound();
                
            return NoContent();
        }
    }

    public class LocationUpdateDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
    }
}