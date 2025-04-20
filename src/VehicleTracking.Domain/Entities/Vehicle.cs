using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace VehicleTracking.Domain.Entities
{
    public class Vehicle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("plateNumber")]
        public string PlateNumber { get; set; } = string.Empty;

        [BsonElement("brand")]
        public string Brand { get; set; } = string.Empty;

        [BsonElement("model")]
        public string Model { get; set; } = string.Empty;

        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("location")]
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; } = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
            new GeoJson2DGeographicCoordinates(0, 0)); // Default to (0,0)

        [BsonElement("speed")]
        public double Speed { get; set; }

        [BsonElement("lastUpdateTime")]
        public DateTime LastUpdateTime { get; set; } = DateTime.UtcNow;

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

        [BsonElement("deviceId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DeviceId { get; set; }

        // Tenant ilişkisi - Araç hangi firmaya (tenant) ait
        [BsonElement("tenantId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TenantId { get; set; }

        [BsonElement("engineRunning")]
        public bool EngineRunning { get; set; }

        // MongoDB'de doğrudan referans olmadığı için bu ilişki uygulama seviyesinde yönetilecek
        [BsonIgnore]
        public Tenant Tenant { get; set; }
    }
}