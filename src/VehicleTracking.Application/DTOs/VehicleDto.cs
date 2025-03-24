using System;

namespace VehicleTracking.Application.DTOs
{
    public class VehicleDto
    {
        public string Id { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public bool IsActive { get; set; }
        public string DeviceId { get; set; } = string.Empty;
    }
} 