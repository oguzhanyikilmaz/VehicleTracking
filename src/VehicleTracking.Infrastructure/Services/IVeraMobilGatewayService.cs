using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VehicleTracking.Infrastructure.Services
{
    public interface IVeraMobilGatewayService
    {
        /// <summary>
        /// Araç konum bilgilerini Veramobil Gateway'e gönderir
        /// </summary>
        /// <param name="vehicleId">Araç ID</param>
        /// <param name="location">Konum bilgisi</param>
        /// <param name="speed">Hız bilgisi (km/s)</param>
        /// <param name="timestamp">Zaman damgası</param>
        /// <returns>İşlemin başarılı olup olmadığı</returns>
        Task<bool> SendLocationDataToGatewayAsync(
            string vehicleId, 
            GeoJsonPoint<GeoJson2DGeographicCoordinates> location, 
            double speed, 
            DateTime timestamp);

        /// <summary>
        /// Verimobil Gateway'e toplu konum verisi gönderme
        /// </summary>
        /// <param name="locationDataList">Gönderilecek konum verileri listesi</param>
        Task SendBulkLocationDataToGatewayAsync(IEnumerable<VehicleLocationData> locationDataList);
    }
} 