using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

                // Örnek veri formatı: "DeviceId,Latitude,Longitude,Speed"
                var parts = rawData.Split(',');
                if (parts.Length != 4)
                {
                    throw new FormatException("Invalid data format");
                }

                var locationData = new TcpLocationData
                {
                    DeviceId = parts[0],
                    Latitude = double.Parse(parts[1]),
                    Longitude = double.Parse(parts[2]),
                    Speed = double.Parse(parts[3]),
                    RawData = rawData
                };

                return locationData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TCP data");
                throw;
            }
        }
    }
} 