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

        /// <summary>
        /// Konum açıklaması - İsteğe bağlı
        /// </summary>
        public string LocationDescription { get; set; }

        /// <summary>
        /// IP adresi - İsteğe bağlı
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Yön bilgisi (derece cinsinden, 0-360) - İsteğe bağlı
        /// </summary>
        public double? Heading { get; set; }

        /// <summary>
        /// Mesafe bilgisi (metre cinsinden) - İsteğe bağlı
        /// </summary>
        public double? Distance { get; set; }

        /// <summary>
        /// Sıcaklık bilgisi (Celsius) - İsteğe bağlı
        /// </summary>
        public double? Temperature { get; set; }

        /// <summary>
        /// Alarm türü (varsa) - İsteğe bağlı
        /// </summary>
        public string AlarmType { get; set; }
    }
} 