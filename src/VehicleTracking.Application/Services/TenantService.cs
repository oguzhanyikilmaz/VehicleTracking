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
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;

        public TenantService(ITenantRepository tenantRepository, IMapper mapper)
        {
            _tenantRepository = tenantRepository;
            _mapper = mapper;
        }

        public async Task<TenantDto> GetTenantByIdAsync(string id)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            return _mapper.Map<TenantDto>(tenant);
        }

        public async Task<List<TenantDto>> GetAllTenantsAsync()
        {
            var tenants = await _tenantRepository.GetAllAsync();
            return _mapper.Map<List<TenantDto>>(tenants);
        }

        public async Task<List<TenantDto>> GetActiveTenantsAsync()
        {
            var tenants = await _tenantRepository.GetActiveTenantsAsync();
            return _mapper.Map<List<TenantDto>>(tenants);
        }

        public async Task<TenantDto> CreateTenantAsync(TenantCreateDto tenantCreateDto)
        {
            // Aynı isimde tenant var mı kontrolü
            if (await _tenantRepository.ExistsByNameAsync(tenantCreateDto.Name))
            {
                throw new InvalidOperationException($"Bu isimde bir kiracı zaten mevcut: {tenantCreateDto.Name}");
            }

            // Entity'ye dönüştür
            var tenant = _mapper.Map<Tenant>(tenantCreateDto);
            
            // Veritabanına ekle
            var createdTenant = await _tenantRepository.AddAsync(tenant);
            
            // DTO'ya dönüştür ve geri döndür
            return _mapper.Map<TenantDto>(createdTenant);
        }

        public async Task<TenantDto> UpdateTenantAsync(TenantUpdateDto tenantUpdateDto)
        {
            // Tenant var mı kontrolü
            var existingTenant = await _tenantRepository.GetByIdAsync(tenantUpdateDto.Id);
            if (existingTenant == null)
            {
                throw new KeyNotFoundException($"Kiracı bulunamadı (ID: {tenantUpdateDto.Id})");
            }

            // İsim değiştiyse ve yeni isimde başka bir tenant varsa hata ver
            if (existingTenant.Name != tenantUpdateDto.Name && 
                await _tenantRepository.ExistsByNameAsync(tenantUpdateDto.Name))
            {
                throw new InvalidOperationException($"Bu isimde bir kiracı zaten mevcut: {tenantUpdateDto.Name}");
            }

            // Değişiklikleri güncelle
            _mapper.Map(tenantUpdateDto, existingTenant);
            
            // Veritabanına kaydet
            var updatedTenant = await _tenantRepository.UpdateAsync(existingTenant);
            
            // DTO'ya dönüştür ve geri döndür
            return _mapper.Map<TenantDto>(updatedTenant);
        }

        public async Task<bool> DeleteTenantAsync(string id)
        {
            // Tenant var mı kontrolü
            if (!await _tenantRepository.ExistsByIdAsync(id))
            {
                throw new KeyNotFoundException($"Kiracı bulunamadı (ID: {id})");
            }

            // Tenant'ın kullanıcı sayısını kontrol et
            var userCount = await _tenantRepository.GetUserCountAsync(id);
            if (userCount > 0)
            {
                throw new InvalidOperationException($"Bu kiracıya ait {userCount} kullanıcı bulunmaktadır. Önce kullanıcıları silmelisiniz.");
            }

            // Tenant'ın araç sayısını kontrol et
            var vehicleCount = await _tenantRepository.GetVehicleCountAsync(id);
            if (vehicleCount > 0)
            {
                throw new InvalidOperationException($"Bu kiracıya ait {vehicleCount} araç bulunmaktadır. Önce araçları silmelisiniz.");
            }

            // Tenant'ın cihaz sayısını kontrol et
            var deviceCount = await _tenantRepository.GetDeviceCountAsync(id);
            if (deviceCount > 0)
            {
                throw new InvalidOperationException($"Bu kiracıya ait {deviceCount} cihaz bulunmaktadır. Önce cihazları silmelisiniz.");
            }

            // Veritabanından sil
            return await _tenantRepository.DeleteAsync(id);
        }

        public async Task<bool> ActivateTenantAsync(string id)
        {
            // Tenant var mı kontrolü
            if (!await _tenantRepository.ExistsByIdAsync(id))
            {
                throw new KeyNotFoundException($"Kiracı bulunamadı (ID: {id})");
            }

            // Tenant'ı aktifleştir
            return await _tenantRepository.ActivateTenantAsync(id);
        }

        public async Task<bool> DeactivateTenantAsync(string id)
        {
            // Tenant var mı kontrolü
            if (!await _tenantRepository.ExistsByIdAsync(id))
            {
                throw new KeyNotFoundException($"Kiracı bulunamadı (ID: {id})");
            }

            // Tenant'ı deaktif et
            return await _tenantRepository.DeactivateTenantAsync(id);
        }

        public async Task<bool> UpdateSubscriptionAsync(string id, TenantSubscriptionUpdateDto subscriptionUpdateDto)
        {
            // Tenant var mı kontrolü
            if (!await _tenantRepository.ExistsByIdAsync(id))
            {
                throw new KeyNotFoundException($"Kiracı bulunamadı (ID: {id})");
            }

            // Abonelik bilgilerini güncelle
            return await _tenantRepository.UpdateSubscriptionAsync(
                id,
                subscriptionUpdateDto.SubscriptionPlan,
                subscriptionUpdateDto.MaxUsers,
                subscriptionUpdateDto.MaxVehicles,
                subscriptionUpdateDto.MaxDevices,
                subscriptionUpdateDto.SubscriptionEndDate);
        }

        public async Task<bool> TenantExistsAsync(string id)
        {
            return await _tenantRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _tenantRepository.ExistsByNameAsync(name);
        }

        public async Task<TenantUsageDto> GetTenantUsageAsync(string id)
        {
            // Tenant var mı kontrolü
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null)
            {
                throw new KeyNotFoundException($"Kiracı bulunamadı (ID: {id})");
            }

            // Kullanım istatistiklerini getir
            var userCount = await _tenantRepository.GetUserCountAsync(id);
            var vehicleCount = await _tenantRepository.GetVehicleCountAsync(id);
            var deviceCount = await _tenantRepository.GetDeviceCountAsync(id);

            // Kullanım istatistiklerini döndür
            return new TenantUsageDto
            {
                TenantId = id,
                TenantName = tenant.Name,
                CurrentUserCount = userCount,
                MaxUsers = tenant.MaxUsers,
                UserPercentage = tenant.MaxUsers > 0 ? (userCount * 100) / tenant.MaxUsers : 0,
                CurrentVehicleCount = vehicleCount,
                MaxVehicles = tenant.MaxVehicles,
                VehiclePercentage = tenant.MaxVehicles > 0 ? (vehicleCount * 100) / tenant.MaxVehicles : 0,
                CurrentDeviceCount = deviceCount,
                MaxDevices = tenant.MaxDevices,
                DevicePercentage = tenant.MaxDevices > 0 ? (deviceCount * 100) / tenant.MaxDevices : 0,
                SubscriptionPlan = tenant.SubscriptionPlan,
                SubscriptionEndDate = tenant.SubscriptionEndDate
            };
        }
    }
} 