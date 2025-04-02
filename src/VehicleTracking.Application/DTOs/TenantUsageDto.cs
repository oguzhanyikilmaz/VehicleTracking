using System;

namespace VehicleTracking.Application.DTOs
{
    public class TenantUsageDto
    {
        public string TenantId { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        
        // Kullanıcı istatistikleri
        public int CurrentUserCount { get; set; }
        public int MaxUsers { get; set; }
        public int UserPercentage { get; set; }
        
        // Araç istatistikleri
        public int CurrentVehicleCount { get; set; }
        public int MaxVehicles { get; set; }
        public int VehiclePercentage { get; set; }
        
        // Cihaz istatistikleri
        public int CurrentDeviceCount { get; set; }
        public int MaxDevices { get; set; }
        public int DevicePercentage { get; set; }
        
        // Abonelik bilgileri
        public string SubscriptionPlan { get; set; } = string.Empty;
        public DateTime? SubscriptionEndDate { get; set; }
    }
    
    public class TenantSubscriptionUpdateDto
    {
        public string SubscriptionPlan { get; set; } = string.Empty;
        public int MaxUsers { get; set; } = 5;
        public int MaxVehicles { get; set; } = 10;
        public int MaxDevices { get; set; } = 10;
        public DateTime? SubscriptionEndDate { get; set; }
    }
} 