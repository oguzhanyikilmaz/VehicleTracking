using System;

namespace VehicleTracking.Domain.Entities
{
    public class UserRole
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public DateTime AssignedAt { get; set; }
        
        // İlişkiler
        public User User { get; set; }
        public Role Role { get; set; }
    }
} 