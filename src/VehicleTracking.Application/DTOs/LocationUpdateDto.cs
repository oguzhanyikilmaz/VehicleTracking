using System;

namespace VehicleTracking.Application.DTOs
{
    /// <summary>
    /// Araç konum güncellemesi için DTO sınıfı
    /// </summary>
    public class LocationUpdateDto
    {
        /// <summary>
        /// Enlem
        /// </summary>
        public double Latitude { get; set; }
        
        /// <summary>
        /// Boylam
        /// </summary>
        public double Longitude { get; set; }
        
        /// <summary>
        /// Aracın hızı (km/saat)
        /// </summary>
        public double Speed { get; set; }
        
        /// <summary>
        /// İsteğe bağlı olarak yön bilgisi (derece cinsinden, 0-360)
        /// </summary>
        public double? Heading { get; set; }
        
        /// <summary>
        /// İsteğe bağlı olarak yükseklik bilgisi (metre cinsinden)
        /// </summary>
        public double? Altitude { get; set; }
        
        /// <summary>
        /// Konum bilgisinin alındığı zaman
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
} 