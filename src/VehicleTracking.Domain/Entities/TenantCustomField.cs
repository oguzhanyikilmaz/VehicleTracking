using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VehicleTracking.Domain.Entities
{
    public class TenantCustomField
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string TenantId { get; set; }
        
        [BsonElement("fieldName")]
        public string FieldName { get; set; }
        
        [BsonElement("fieldType")]
        public string FieldType { get; set; } // String, Number, Date, Boolean, etc.
        
        [BsonElement("fieldValue")]
        public string FieldValue { get; set; }
        
        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
        
        [BsonIgnore]
        public Tenant Tenant { get; set; }
    }
} 