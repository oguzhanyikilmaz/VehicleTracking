namespace VehicleTracking.Infrastructure.TCP.Models
{
    /// <summary>
    /// TCP'den alınan konum verilerini içeren model
    /// </summary>
    public class TcpLocationData
    {
        /// <summary>
        /// Cihaz ID
        /// </summary>
        public string DeviceId { get; set; }
        
        /// <summary>
        /// Enlem
        /// </summary>
        public double Latitude { get; set; }
        
        /// <summary>
        /// Boylam
        /// </summary>
        public double Longitude { get; set; }
        
        /// <summary>
        /// Hız (km/saat)
        /// </summary>
        public double Speed { get; set; }
        
        /// <summary>
        /// Alınan ham veri
        /// </summary>
        public string RawData { get; set; }
        
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