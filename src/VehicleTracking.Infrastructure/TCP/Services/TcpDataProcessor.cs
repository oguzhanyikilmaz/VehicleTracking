using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Linq;
using System.Globalization;
using VehicleTracking.Infrastructure.TCP.Interfaces;
using VehicleTracking.Infrastructure.TCP.Models;

namespace VehicleTracking.Infrastructure.TCP.Services
{
    public class TcpDataProcessor : ITcpDataProcessor
    {
        private readonly ILogger<TcpDataProcessor> _logger;

        public TcpDataProcessor(ILogger<TcpDataProcessor> logger)
        {
            _logger = logger;
        }

        public async Task<TcpLocationData> ProcessDataAsync(string rawData)
        {
            try
            {
                _logger.LogInformation($"Processing TCP data: {rawData}");

                // Geniş protokol formatı:
                // "DeviceId,Latitude,Longitude,Speed,LocationDesc,IpAddress,Heading,Distance,Temperature,AlarmType"
                // Ancak, daha eski cihazlar daha az alan gönderebilir, eksik alanlar için null kullanacağız
                
                var parts = rawData.Split(',');
                if (parts.Length < 4)
                {
                    throw new FormatException($"Invalid data format. Expected at least 4 fields, but got {parts.Length}");
                }

                // Temel alanlar (zorunlu)
                var locationData = new TcpLocationData
                {
                    DeviceId = parts[0],
                    Latitude = ParseDouble(parts[1]),
                    Longitude = ParseDouble(parts[2]),
                    Speed = ParseDouble(parts[3]),
                    RawData = rawData
                };

                // Opsiyonel alanlar - cihazdan gelirlerse bunları da dolduralım
                if (parts.Length > 4 && !string.IsNullOrWhiteSpace(parts[4]))
                    locationData.LocationDescription = parts[4];
                
                if (parts.Length > 5 && !string.IsNullOrWhiteSpace(parts[5]))
                    locationData.IpAddress = parts[5];
                
                if (parts.Length > 6 && !string.IsNullOrWhiteSpace(parts[6]))
                    locationData.Heading = ParseDouble(parts[6]);
                
                if (parts.Length > 7 && !string.IsNullOrWhiteSpace(parts[7]))
                    locationData.Distance = ParseDouble(parts[7]);
                
                if (parts.Length > 8 && !string.IsNullOrWhiteSpace(parts[8]))
                    locationData.Temperature = ParseDouble(parts[8]);
                
                if (parts.Length > 9 && !string.IsNullOrWhiteSpace(parts[9]))
                    locationData.AlarmType = parts[9];

                // İstemci IP adresini almak için alternatif yöntem: alanı cihazdan gelmezse, veride '$IP$' özel etiketi olabilir
                if (string.IsNullOrWhiteSpace(locationData.IpAddress) && rawData.Contains("$IP$"))
                {
                    var ipParts = rawData.Split(new[] { "$IP$" }, StringSplitOptions.None);
                    if (ipParts.Length > 1)
                    {
                        var ipCandidate = ipParts[1].Trim();
                        // IP adresinin geçerli olup olmadığını kontrol et
                        if (IPAddress.TryParse(ipCandidate, out _))
                        {
                            locationData.IpAddress = ipCandidate;
                        }
                    }
                }

                // Cihaz verilerini analiz ettikten sonra, bazı çıkarımlar yapabiliriz:
                
                // 1. Konum açıklaması boşsa ve koordinatlar varsa, tersine geocoding yapılabilir (burada basit bir örnek veriyoruz)
                if (string.IsNullOrWhiteSpace(locationData.LocationDescription))
                {
                    locationData.LocationDescription = $"Konum: {locationData.Latitude}, {locationData.Longitude}";
                }

                // 2. Alarm durumunu kontrol et (örneğin hız sınırı aşımı)
                if (string.IsNullOrWhiteSpace(locationData.AlarmType) && locationData.Speed > 120)
                {
                    locationData.AlarmType = "OVERSPEED";
                }

                _logger.LogDebug("Processed location data: DeviceId={DeviceId}, Location=({Lat},{Lon}), Speed={Speed}, Additional fields processed={AdditionalFields}",
                    locationData.DeviceId, locationData.Latitude, locationData.Longitude, locationData.Speed, parts.Length - 4);

                return locationData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TCP data: {RawData}", rawData);
                throw;
            }
        }

        // Farklı formatları desteklemek için yardımcı metod
        private double ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            // Farklı kültürlerdeki ondalık ayırıcıları destekle (. veya ,)
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return result;

            if (double.TryParse(value, NumberStyles.Any, new CultureInfo("tr-TR"), out result))
                return result;

            return 0;
        }
    }
} 