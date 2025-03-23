using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Application.Services;

namespace VehicleTracking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAllVehicles()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleDto>> GetVehicle(Guid id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            return Ok(vehicle);
        }

        [HttpPost]
        public async Task<ActionResult<VehicleDto>> CreateVehicle(VehicleDto vehicleDto)
        {
            var result = await _vehicleService.CreateVehicleAsync(vehicleDto);
            return CreatedAtAction(nameof(GetVehicle), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(Guid id, VehicleDto vehicleDto)
        {
            if (id != vehicleDto.Id)
                return BadRequest();

            await _vehicleService.UpdateVehicleAsync(vehicleDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            await _vehicleService.DeleteVehicleAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/location")]
        public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] LocationUpdateDto location)
        {
            await _vehicleService.UpdateVehicleLocationAsync(id, location.Latitude, location.Longitude, location.Speed);
            return NoContent();
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetActiveVehicles()
        {
            var vehicles = await _vehicleService.GetActiveVehiclesAsync();
            return Ok(vehicles);
        }
    }

    public class LocationUpdateDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
    }
} 