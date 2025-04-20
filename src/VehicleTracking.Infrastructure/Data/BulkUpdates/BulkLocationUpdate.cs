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
        
        /// <summary>
        /// Konum açıklaması
        /// </summary>
        public string LocationDescription { get; set; }
        
        /// <summary>
        /// IP adresi
        /// </summary>
        public string IpAddress { get; set; }
        
        /// <summary>
        /// Yön bilgisi (derece, 0-360)
        /// </summary>
        public double? Heading { get; set; }
        
        /// <summary>
        /// Kat edilen mesafe (metre)
        /// </summary>
        public double? Distance { get; set; }
        
        /// <summary>
        /// Sıcaklık (Celsius)
        /// </summary>
        public double? Temperature { get; set; }
        
        /// <summary>
        /// Alarm tipi
        /// </summary>
        public string AlarmType { get; set; }
    }
} 