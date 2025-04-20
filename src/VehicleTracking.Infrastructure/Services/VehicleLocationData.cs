using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace VehicleTracking.Infrastructure.Services
{
    /// <summary>
    /// Araç konum verisi modeli - Gateway servislerine gönderilecek veriler için kullanılır
    /// </summary>
    public class VehicleLocationData
    {
        /// <summary>
        /// Araç ID'si
        /// </summary>
        public string VehicleId { get; set; }
        
        /// <summary>
        /// Konum bilgisi (GeoJSON formatında)
        /// </summary>
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; }
        
        /// <summary>
        /// Hız bilgisi (km/s)
        /// </summary>
        public double Speed { get; set; }
        
        /// <summary>
        /// Zaman damgası
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
} 