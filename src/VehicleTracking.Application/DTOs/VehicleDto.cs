using System;

namespace VehicleTracking.Application.DTOs
{
    public class VehicleDto
    {
        public Guid Id { get; set; }
        public string PlateNumber { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public bool IsActive { get; set; }
        public string DeviceId { get; set; }
    }
} 