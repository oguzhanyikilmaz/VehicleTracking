using System;

namespace VehicleTracking.Infrastructure.Data.BulkUpdates
{
    public class BulkLocationUpdate
    {
        public Guid VehicleId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 