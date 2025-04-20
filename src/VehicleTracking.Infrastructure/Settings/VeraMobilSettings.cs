namespace VehicleTracking.Infrastructure.Settings
{
    /// <summary>
    /// Veramobil Gateway API ayarları
    /// </summary>
    public class VeraMobilSettings
    {
        /// <summary>
        /// Gateway API URL'i
        /// </summary>
        public string GatewayUrl { get; set; } = "https://t10.veramobil.com.tr/gateway/";
        
        /// <summary>
        /// API anahtarı
        /// </summary>
        public string ApiKey { get; set; } = "vera2025";
        
        /// <summary>
        /// HTTP istemcisi zamanaşımı süresi (saniye)
        /// </summary>
        public int TimeoutSeconds { get; set; } = 15;
        
        /// <summary>
        /// Varsayılan IP adresi
        /// </summary>
        public string DefaultIpAddress { get; set; } = "1.1.1.1";
        
        /// <summary>
        /// Varsayılan konum açıklaması
        /// </summary>
        public string DefaultLocationDescription { get; set; } = "Şirinyalı Mahallesi, Muratpaşa, Antalya, Türkiye";
    }
} 