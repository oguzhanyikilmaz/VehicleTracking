using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VehicleTracking.Domain.Entities
{
    public class Tenant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        
        [BsonElement("name")]
        public string Name { get; set; }
        
        [BsonElement("description")]
        public string Description { get; set; }
        
        [BsonElement("email")]
        public string Email { get; set; }
        
        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }
        
        [BsonElement("address")]
        public string Address { get; set; }
        
        [BsonElement("taxId")]
        public string TaxId { get; set; }
        
        [BsonElement("contactPerson")]
        public string ContactPerson { get; set; }
        
        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
        
        [BsonElement("subscriptionPlan")]
        public string SubscriptionPlan { get; set; } = "Free";
        
        [BsonElement("subscriptionStartDate")]
        public DateTime SubscriptionStartDate { get; set; } = DateTime.UtcNow;
        
        [BsonElement("subscriptionEndDate")]
        public DateTime? SubscriptionEndDate { get; set; }
        
        [BsonElement("maxUsers")]
        public int MaxUsers { get; set; } = 5;
        
        [BsonElement("maxVehicles")]
        public int MaxVehicles { get; set; } = 10;
        
        [BsonElement("maxDevices")]
        public int MaxDevices { get; set; } = 10;
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
        
        [BsonElement("logo")]
        public string Logo { get; set; }
        
        [BsonElement("website")]
        public string Website { get; set; }
    }
} 