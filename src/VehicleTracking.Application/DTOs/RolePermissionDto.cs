using System;

namespace VehicleTracking.Application.DTOs
{
    /// <summary>
    /// Rol ve izin ilişkisi için DTO sınıfı
    /// </summary>
    public class RolePermissionDto
    {
        /// <summary>
        /// Rol ID
        /// </summary>
        public string RoleId { get; set; } = string.Empty;
        
        /// <summary>
        /// İzin ID
        /// </summary>
        public string PermissionId { get; set; } = string.Empty;
    }
} 