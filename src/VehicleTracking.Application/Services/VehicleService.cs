using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Driver.GeoJsonObjectModel;
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

        public async Task<VehicleDto> GetVehicleByIdAsync(string id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
        }

        public async Task<VehicleDto> AddVehicleAsync(VehicleDto vehicleDto)
        {
            var vehicle = _mapper.Map<Vehicle>(vehicleDto);
            var result = await _vehicleRepository.AddAsync(vehicle);
            return _mapper.Map<VehicleDto>(result);
        }

        public async Task<VehicleDto> UpdateVehicleAsync(VehicleDto vehicleDto)
        {
            var vehicle = _mapper.Map<Vehicle>(vehicleDto);
            var updated = await _vehicleRepository.UpdateAsync(vehicle);
            return _mapper.Map<VehicleDto>(updated);
        }

        public async Task<bool> DeleteVehicleAsync(string id)
        {
            return await _vehicleRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdateVehicleLocationAsync(string id, double latitude, double longitude, double speed)
        {
            var geoPoint = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                new GeoJson2DGeographicCoordinates(longitude, latitude));
                
            return await _vehicleRepository.UpdateLocationAsync(id, geoPoint, speed);
        }

        public async Task<IEnumerable<VehicleDto>> GetActiveVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetActiveVehiclesAsync();
            return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
        }

        public async Task<IEnumerable<VehicleDto>> GetVehiclesByIdsAsync(IEnumerable<string> ids)
        {
            var vehicles = await _vehicleRepository.GetVehiclesByIdsAsync(ids);
            return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
        }

        public async Task<IEnumerable<VehicleDto>> GetVehiclesNearLocationAsync(double latitude, double longitude, double maxDistanceKm)
        {
            var vehicles = await _vehicleRepository.GetVehiclesNearLocationAsync(latitude, longitude, maxDistanceKm);
            return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
        }

        public async Task<VehicleDto> GetVehicleByDeviceIdAsync(string deviceId)
        {
            var vehicle = await _vehicleRepository.GetVehicleByDeviceIdAsync(deviceId);
            return _mapper.Map<VehicleDto>(vehicle);
        }
    }
} 