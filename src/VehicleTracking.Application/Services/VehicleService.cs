using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;

namespace VehicleTracking.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;

        public VehicleService(IVehicleRepository vehicleRepository, IMapper mapper)
        {
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
        }

        public async Task<VehicleDto> GetVehicleByIdAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
        }

        public async Task<VehicleDto> CreateVehicleAsync(VehicleDto vehicleDto)
        {
            var vehicle = _mapper.Map<Vehicle>(vehicleDto);
            var result = await _vehicleRepository.AddAsync(vehicle);
            return _mapper.Map<VehicleDto>(result);
        }

        public async Task UpdateVehicleAsync(VehicleDto vehicleDto)
        {
            var vehicle = _mapper.Map<Vehicle>(vehicleDto);
            await _vehicleRepository.UpdateAsync(vehicle);
        }

        public async Task DeleteVehicleAsync(Guid id)
        {
            await _vehicleRepository.DeleteAsync(id);
        }

        public async Task UpdateVehicleLocationAsync(Guid id, double latitude, double longitude, double speed)
        {
            await _vehicleRepository.UpdateLocationAsync(id, latitude, longitude, speed);
        }

        public async Task<IEnumerable<VehicleDto>> GetActiveVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetActiveVehiclesAsync();
            return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
        }

        public async Task<IEnumerable<VehicleDto>> GetVehiclesByIdsAsync(IEnumerable<Guid> ids)
        {
            var vehicles = await _vehicleRepository.GetVehiclesByIdsAsync(ids);
            return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
        }
    }
} 