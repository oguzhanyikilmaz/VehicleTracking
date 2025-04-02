using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleTracking.Application.DTOs
{
    /// <summary>
    /// Kullanıcı ve rol ilişkisi için DTO sınıfı
    /// </summary>
    public class UserRoleDto
    {
        /// <summary>
        /// Kullanıcı ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        
        /// <summary>
        /// Rol ID
        /// </summary>
        public string RoleId { get; set; } = string.Empty;
    }
}
