using System;

namespace VehicleTracking.Application.DTOs
{
    public class DeviceUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string FirmwareVersion { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string SimCardNumber { get; set; } = string.Empty;
        public string CommunicationType { get; set; } = "TCP";
        public string Notes { get; set; } = string.Empty;
        public string DeviceType { get; set; } = "GPS";
        public string Manufacturer { get; set; } = string.Empty;
        public DateTime? WarrantlyExpireDate { get; set; }
    }
} 