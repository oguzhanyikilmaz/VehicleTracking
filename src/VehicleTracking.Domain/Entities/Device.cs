using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VehicleTracking.Domain.Entities
{
    public class Device
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        
        [BsonElement("serialNumber")]
        public string SerialNumber { get; set; }
        
        [BsonElement("deviceModel")]
        public string Model { get; set; }
        
        [BsonElement("firmwareVersion")]
        public string FirmwareVersion { get; set; }
        
        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
        
        [BsonElement("activationDate")]
        public DateTime ActivationDate { get; set; } = DateTime.UtcNow;
        
        [BsonElement("lastConnectionTime")]
        public DateTime LastConnectionTime { get; set; } = DateTime.UtcNow;
        
        [BsonElement("vehicleId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VehicleId { get; set; }
        
        [BsonElement("tenantId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TenantId { get; set; }
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
        
        [BsonElement("simCardNumber")]
        public string SimCardNumber { get; set; }
        
        [BsonElement("ipAddress")]
        public string IpAddress { get; set; }
        
        [BsonElement("port")]
        public int? Port { get; set; }
        
        [BsonElement("communicationType")]
        public string CommunicationType { get; set; } = "TCP";
        
        [BsonElement("notes")]
        public string Notes { get; set; }
        
        [BsonElement("deviceType")]
        public string DeviceType { get; set; } = "GPS";
        
        [BsonElement("manufacturer")]
        public string Manufacturer { get; set; }
        
        [BsonElement("warrantlyExpireDate")]
        public DateTime? WarrantlyExpireDate { get; set; }
        
        [BsonIgnore]
        public Vehicle Vehicle { get; set; }
        
        [BsonIgnore]
        public Tenant Tenant { get; set; }
    }
} 