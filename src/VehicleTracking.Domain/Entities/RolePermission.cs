using System;

namespace VehicleTracking.Domain.Entities
{
    public class RolePermission
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string PermissionId { get; set; }
        public DateTime AssignedAt { get; set; }
        
        // İlişkiler
        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }
} 