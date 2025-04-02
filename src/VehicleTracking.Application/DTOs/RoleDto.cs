using System;
using System.Collections.Generic;

namespace VehicleTracking.Application.DTOs
{
    public class RoleDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // İlişkili izinler
        public List<string> PermissionIds { get; set; } = new List<string>();
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }
    
    // Rol oluşturma için DTO
    public class RoleCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public List<string> PermissionIds { get; set; } = new List<string>();
    }
    
    // Rol güncelleme için DTO
    public class RoleUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
} 