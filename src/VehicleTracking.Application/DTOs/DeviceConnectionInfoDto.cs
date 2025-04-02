using System;

namespace VehicleTracking.Application.DTOs
{
    /// <summary>
    /// Cihaz bağlantı bilgileri DTO sınıfı
    /// </summary>
    public class DeviceConnectionInfoDto
    {
        /// <summary>
        /// Cihazın IP adresi
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// Cihazın bağlantı portu
        /// </summary>
        public int Port { get; set; }
        
        /// <summary>
        /// Cihazın iletişim türü (TCP, UDP, MQTT, vb.)
        /// </summary>
        public string CommunicationType { get; set; } = string.Empty;
    }
} 