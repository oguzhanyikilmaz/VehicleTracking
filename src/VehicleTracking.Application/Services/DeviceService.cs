using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;
using System.Linq;

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

        public async Task<IEnumerable<DeviceDto>> GetUnassignedDevicesByTenantIdAsync(string tenantId)
        {
            // Tenant ID için kontrol
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new ArgumentNullException(nameof(tenantId), "Kiracı ID boş olamaz");
            }

            // Kiracının var olduğunu kontrol et
            if (!await _tenantRepository.ExistsByIdAsync(tenantId))
            {
                throw new KeyNotFoundException($"Kiracı bulunamadı (ID: {tenantId})");
            }

            // Repository üzerinden doğrudan çağır
            var devices = await _deviceRepository.GetUnassignedDevicesByTenantIdAsync(tenantId);
            return _mapper.Map<IEnumerable<DeviceDto>>(devices);
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

        public async Task<DeviceDto> ActivateDeviceAsync(string id)
        {
            var device = await _deviceRepository.GetByIdAsync(id);
            if (device == null)
            {
                throw new KeyNotFoundException($"Cihaz bulunamadı (ID: {id})");
            }

            var result = await _deviceRepository.ActivateAsync(id);
            if (!result)
            {
                throw new InvalidOperationException($"Cihaz aktifleştirilemedi (ID: {id})");
            }

            // Güncellenmiş cihazı getir
            device = await _deviceRepository.GetByIdAsync(id);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<DeviceDto> DeactivateDeviceAsync(string id)
        {
            var device = await _deviceRepository.GetByIdAsync(id);
            if (device == null)
            {
                throw new KeyNotFoundException($"Cihaz bulunamadı (ID: {id})");
            }

            var result = await _deviceRepository.DeactivateAsync(id);
            if (!result)
            {
                throw new InvalidOperationException($"Cihaz deaktifleştirilemedi (ID: {id})");
            }

            // Güncellenmiş cihazı getir
            device = await _deviceRepository.GetByIdAsync(id);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<DeviceDto> AssignDeviceToVehicleAsync(string deviceId, string vehicleId)
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
            var assignResult = await _deviceRepository.AssignToVehicleAsync(deviceId, vehicleId);
            var updateResult = await _vehicleRepository.UpdateDeviceIdAsync(vehicleId, deviceId);

            if (!assignResult || !updateResult)
            {
                throw new InvalidOperationException("Cihaz araca atanamadı");
            }

            // Güncellenmiş cihazı getir
            device = await _deviceRepository.GetByIdAsync(deviceId);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<DeviceDto> UnassignDeviceFromVehicleAsync(string deviceId)
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

            var result = await _deviceRepository.RemoveFromVehicleAsync(deviceId);
            if (!result)
            {
                throw new InvalidOperationException($"Cihaz araçtan ayrılamadı (ID: {deviceId})");
            }

            // Güncellenmiş cihazı getir
            device = await _deviceRepository.GetByIdAsync(deviceId);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<DeviceDto> UpdateDeviceConnectionInfoAsync(string id, string ipAddress, int port, string communicationType)
        {
            var device = await _deviceRepository.GetByIdAsync(id);
            if (device == null)
            {
                throw new KeyNotFoundException($"Cihaz bulunamadı (ID: {id})");
            }

            // Cihazın bağlantı bilgilerini güncelle
            device.IpAddress = ipAddress;
            device.Port = port;
            device.CommunicationType = communicationType;
            device.LastConnectionTime = DateTime.UtcNow;
            device.UpdatedAt = DateTime.UtcNow;

            await _deviceRepository.UpdateAsync(device);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<DeviceDto> GetDeviceByVehicleIdAsync(string vehicleId)
        {
            var device = await _deviceRepository.GetByVehicleIdAsync(vehicleId).ContinueWith(t => t.Result.FirstOrDefault());
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<bool> DeviceExistsAsync(string id)
        {
            return await _deviceRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> SerialNumberExistsAsync(string serialNumber)
        {
            return await _deviceRepository.ExistsBySerialNumberAsync(serialNumber);
        }

        public async Task<DeviceDto> UpdateLastConnectionTimeAsync(string id)
        {
            var device = await _deviceRepository.GetByIdAsync(id);
            if (device == null)
            {
                throw new KeyNotFoundException($"Cihaz bulunamadı (ID: {id})");
            }

            // Son bağlantı zamanını güncelle
            device.LastConnectionTime = DateTime.UtcNow;
            device.UpdatedAt = DateTime.UtcNow;

            await _deviceRepository.UpdateAsync(device);
            return _mapper.Map<DeviceDto>(device);
        }
    }
} 