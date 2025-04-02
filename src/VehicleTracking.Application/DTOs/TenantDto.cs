using System;
using System.Collections.Generic;

namespace VehicleTracking.Application.DTOs
{
    public class TenantDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // İletişim bilgileri
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        
        // Diğer firma detayları
        public string TaxId { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        
        // Abonelik bilgileri
        public string SubscriptionPlan { get; set; } = "Free";
        public DateTime SubscriptionStartDate { get; set; } = DateTime.UtcNow;
        public DateTime? SubscriptionEndDate { get; set; }
        public int MaxUsers { get; set; } = 5;
        public int MaxVehicles { get; set; } = 10;
        public int MaxDevices { get; set; } = 10;
        
        // Özel alanlar - Bu dictionary, tenant'a ait dinamik özel alanları tutar
        public Dictionary<string, string> CustomFields { get; set; } = new Dictionary<string, string>();
    }
    
    public class TenantCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string SubscriptionPlan { get; set; } = "Free";
    }

    public class TenantUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string SubscriptionPlan { get; set; } = "Free";
        public int MaxUsers { get; set; } = 5;
        public int MaxVehicles { get; set; } = 10;
        public int MaxDevices { get; set; } = 10;
        public DateTime? SubscriptionEndDate { get; set; }
    }
} 