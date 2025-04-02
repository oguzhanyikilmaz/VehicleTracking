using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VehicleTracking.Domain.Entities
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        
        [BsonElement("username")]
        public string Username { get; set; }
        
        [BsonElement("email")]
        public string Email { get; set; }
        
        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }
        
        [BsonElement("firstName")]
        public string FirstName { get; set; }
        
        [BsonElement("lastName")]
        public string LastName { get; set; }
        
        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }
        
        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
        
        [BsonElement("refreshToken")]
        public string RefreshToken { get; set; }
        
        [BsonElement("refreshTokenExpiryTime")]
        public DateTime? RefreshTokenExpiryTime { get; set; }
        
        [BsonElement("emailConfirmed")]
        public bool EmailConfirmed { get; set; } = false;
        
        [BsonElement("lastLoginAt")]
        public DateTime? LastLoginAt { get; set; }
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
        
        [BsonElement("tenantId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TenantId { get; set; }
        
        [BsonElement("roleIds")]
        public List<string> RoleIds { get; set; } = new List<string>();
        
        [BsonElement("language")]
        public string Language { get; set; } = "tr";
        
        [BsonElement("theme")]
        public string Theme { get; set; } = "light";
        
        [BsonIgnore]
        public Tenant Tenant { get; set; }
        
        [BsonIgnore]
        public List<Role> Roles { get; set; } = new List<Role>();
        
        [BsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
    }
} 