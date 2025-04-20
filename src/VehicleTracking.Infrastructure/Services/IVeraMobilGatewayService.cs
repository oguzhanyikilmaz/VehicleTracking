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
        /// <param name="locationDescription">Konum açıklaması (isteğe bağlı)</param>
        /// <param name="ipAddress">IP adresi (isteğe bağlı)</param>
        /// <param name="heading">Yön bilgisi (derece cinsinden, 0-360) (isteğe bağlı)</param>
        /// <param name="distance">Mesafe bilgisi (metre cinsinden) (isteğe bağlı)</param>
        /// <param name="temperature">Sıcaklık bilgisi (Celsius) (isteğe bağlı)</param>
        /// <param name="alarmType">Alarm türü (isteğe bağlı)</param>
        /// <returns>İşlemin başarılı olup olmadığı</returns>
        Task<bool> SendLocationDataToGatewayAsync(
            string vehicleId, 
            GeoJsonPoint<GeoJson2DGeographicCoordinates> location, 
            double speed, 
            DateTime timestamp,
            string locationDescription = null,
            string ipAddress = null,
            double? heading = null,
            double? distance = null,
            double? temperature = null,
            string alarmType = null);

        /// <summary>
        /// Verimobil Gateway'e toplu konum verisi gönderme
        /// </summary>
        /// <param name="locationDataList">Gönderilecek konum verileri listesi</param>
        Task SendBulkLocationDataToGatewayAsync(IEnumerable<VehicleLocationData> locationDataList);
    }
} 