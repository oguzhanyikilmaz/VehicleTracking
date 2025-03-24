using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace VehicleTracking.Infrastructure.Data.BulkUpdates
{
    public class BulkLocationUpdate
    {
        public string VehicleId { get; set; } = string.Empty;
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; } = 
            new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(0, 0));
        public double Speed { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
} 