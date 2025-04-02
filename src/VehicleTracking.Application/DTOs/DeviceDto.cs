using System;

namespace VehicleTracking.Application.DTOs
{
    public class DeviceDto
    {
        public string Id { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string FirmwareVersion { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime ActivationDate { get; set; } = DateTime.UtcNow;
        public DateTime LastConnectionTime { get; set; } = DateTime.UtcNow;
        public string VehicleId { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string SimCardNumber { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int? Port { get; set; }
        public string CommunicationType { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public DateTime? WarrantlyExpireDate { get; set; }
    }
} 