using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;

namespace VehicleTracking.Application.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;

        public DeviceService(
            IDeviceRepository deviceRepository,
            IVehicleRepository vehicleRepository,
            ITenantRepository tenantRepository,
            IMapper mapper)
        {
            _deviceRepository = deviceRepository;
            _vehicleRepository = vehicleRepository;
            _tenantRepository = tenantRepository;
            _mapper = mapper;
        }

        public async Task<DeviceDto> GetDeviceByIdAsync(string id)
        {
            var device = await _deviceRepository.GetByIdAsync(id);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<DeviceDto> GetDeviceBySerialNumberAsync(string serialNumber)
        {
            var device = await _deviceRepository.GetBySerialNumberAsync(serialNumber);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<List<DeviceDto>> GetAllDevicesAsync()
        {
            var devices = await _deviceRepository.GetAllAsync();
            return _mapper.Map<List<DeviceDto>>(devices);
        }

        public async Task<List<DeviceDto>> GetActiveDevicesAsync()
        {
            var devices = await _deviceRepository.GetActiveDevicesAsync();
            return _mapper.Map<List<DeviceDto>>(devices);
        }

        public async Task<List<DeviceDto>> GetDevicesByTenantIdAsync(string tenantId)
        {
            var devices = await _deviceRepository.GetByTenantIdAsync(tenantId);
            return _mapper.Map<List<DeviceDto>>(devices);
        }

        public async Task<List<DeviceDto>> GetUnassignedDevicesAsync()
        {
            var devices = await _deviceRepository.GetUnassignedDevicesAsync();
            return _mapper.Map<List<DeviceDto>>(devices);
        }

        public async Task<List<DeviceDto>> GetUnassignedDevicesByTenantIdAsync(string tenantId)
        {
            var devices = await _deviceRepository.GetUnassignedDevicesByTenantIdAsync(tenantId);
            return _mapper.Map<List<DeviceDto>>(devices);
        }

        public async Task<DeviceDto> CreateDeviceAsync(DeviceCreateDto deviceCreateDto)
        {
            // Seri numarası kontrolü
            if (await _deviceRepository.ExistsBySerialNumberAsync(deviceCreateDto.SerialNumber))
            {
                throw new InvalidOperationException($"'{deviceCreateDto.SerialNumber}' seri numaralı cihaz zaten mevcut");
            }

            // Kiracı kontrolü
            if (!string.IsNullOrEmpty(deviceCreateDto.TenantId) && !await _tenantRepository.ExistsByIdAsync(deviceCreateDto.TenantId))
            {
                throw new KeyNotFoundException($"Kiracı bulunamadı (ID: {deviceCreateDto.TenantId})");
            }

            // Tenanta ait maksimum cihaz sayısı kontrolü
            if (!string.IsNullOrEmpty(deviceCreateDto.TenantId))
            {
                var tenant = await _tenantRepository.GetByIdAsync(deviceCreateDto.TenantId);
                int currentDeviceCount = await _deviceRepository.GetDeviceCountByTenantIdAsync(deviceCreateDto.TenantId);

                if (tenant != null && currentDeviceCount >= tenant.MaxDevices)
                {
                    throw new InvalidOperationException($"Kiracı ({tenant.Name}) maksimum cihaz limitine ({tenant.MaxDevices}) ulaştı");
                }
            }

            var device = _mapper.Map<Device>(deviceCreateDto);
            device.IsActive = true;
            device.ActivationDate = DateTime.UtcNow;
            device.CreatedAt = DateTime.UtcNow;

            device = await _deviceRepository.AddAsync(device);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<DeviceDto> UpdateDeviceAsync(DeviceUpdateDto deviceUpdateDto)
        {
            var device = await _deviceRepository.GetByIdAsync(deviceUpdateDto.Id);
            if (device == null)
            {
                throw new KeyNotFoundException($"Cihaz bulunamadı (ID: {deviceUpdateDto.Id})");
            }

            // Temel özellikler güncelleniyor
            device.Model = deviceUpdateDto.Model;
            device.FirmwareVersion = deviceUpdateDto.FirmwareVersion;
            device.IsActive = deviceUpdateDto.IsActive;
            device.UpdatedAt = DateTime.UtcNow;
            device.SimCardNumber = deviceUpdateDto.SimCardNumber;
            device.CommunicationType = deviceUpdateDto.CommunicationType;
            device.Notes = deviceUpdateDto.Notes;
            device.DeviceType = deviceUpdateDto.DeviceType;
            device.Manufacturer = deviceUpdateDto.Manufacturer;
            device.WarrantlyExpireDate = deviceUpdateDto.WarrantlyExpireDate;

            device = await _deviceRepository.UpdateAsync(device);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<bool> DeleteDeviceAsync(string id)
        {
            var device = await _deviceRepository.GetByIdAsync(id);
            if (device == null)
            {
                return false;
            }

            // Cihaz bir araca atanmışsa, önce araca olan bağlantıyı kaldır
            if (!string.IsNullOrEmpty(device.VehicleId))
            {
                await _vehicleRepository.UpdateDeviceIdAsync(device.VehicleId, null);
            }

            return await _deviceRepository.DeleteAsync(id);
        }

        public async Task<bool> ActivateDeviceAsync(string id)
        {
            if (!await _deviceRepository.ExistsByIdAsync(id))
            {
                throw new KeyNotFoundException($"Cihaz bulunamadı (ID: {id})");
            }

            return await _deviceRepository.ActivateAsync(id);
        }

        public async Task<bool> DeactivateDeviceAsync(string id)
        {
            if (!await _deviceRepository.ExistsByIdAsync(id))
            {
                throw new KeyNotFoundException($"Cihaz bulunamadı (ID: {id})");
            }

            return await _deviceRepository.DeactivateAsync(id);
        }

        public async Task<bool> AssignDeviceToVehicleAsync(string deviceId, string vehicleId)
        {
            // Cihaz kontrolü
            var device = await _deviceRepository.GetByIdAsync(deviceId);
            if (device == null)
            {
                throw new KeyNotFoundException($"Cihaz bulunamadı (ID: {deviceId})");
            }

            // Araç kontrolü
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
            {
                throw new KeyNotFoundException($"Araç bulunamadı (ID: {vehicleId})");
            }

            // Aracın zaten bir cihazı varsa, eski cihazı araçtan kaldır
            if (!string.IsNullOrEmpty(vehicle.DeviceId))
            {
                await _deviceRepository.RemoveFromVehicleAsync(vehicle.DeviceId);
            }

            // Cihazın atandığı bir araç varsa, o araçtan cihazı kaldır
            if (!string.IsNullOrEmpty(device.VehicleId) && device.VehicleId != vehicleId)
            {
                await _vehicleRepository.UpdateDeviceIdAsync(device.VehicleId, null);
            }

            // Cihazı araca, aracı cihaza atama işlemleri
            await _deviceRepository.AssignToVehicleAsync(deviceId, vehicleId);
            await _vehicleRepository.UpdateDeviceIdAsync(vehicleId, deviceId);

            return true;
        }

        public async Task<bool> RemoveDeviceFromVehicleAsync(string deviceId)
        {
            var device = await _deviceRepository.GetByIdAsync(deviceId);
            if (device == null)
            {
                throw new KeyNotFoundException($"Cihaz bulunamadı (ID: {deviceId})");
            }

            // Cihazın atandığı araç varsa, araçtan da cihazı kaldır
            if (!string.IsNullOrEmpty(device.VehicleId))
            {
                await _vehicleRepository.UpdateDeviceIdAsync(device.VehicleId, null);
            }

            return await _deviceRepository.RemoveFromVehicleAsync(deviceId);
        }

        public async Task<bool> UpdateConnectionInfoAsync(string id, string ipAddress, int port)
        {
            if (!await _deviceRepository.ExistsByIdAsync(id))
            {
                throw new KeyNotFoundException($"Cihaz bulunamadı (ID: {id})");
            }

            return await _deviceRepository.UpdateConnectionInfoAsync(id, ipAddress, port);
        }

        public async Task<bool> UpdateLastConnectionTimeAsync(string id)
        {
            if (!await _deviceRepository.ExistsByIdAsync(id))
            {
                throw new KeyNotFoundException($"Cihaz bulunamadı (ID: {id})");
            }

            return await _deviceRepository.UpdateLastConnectionTimeAsync(id);
        }

        public async Task<bool> DeviceExistsAsync(string id)
        {
            return await _deviceRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> SerialNumberExistsAsync(string serialNumber)
        {
            return await _deviceRepository.ExistsBySerialNumberAsync(serialNumber);
        }
    }
} 