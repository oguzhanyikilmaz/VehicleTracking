using System;

namespace VehicleTracking.Application.DTOs
{
    /// <summary>
    /// Şifre sıfırlama isteği için DTO sınıfı
    /// </summary>
    public class PasswordResetRequestDto
    {
        /// <summary>
        /// Kullanıcının e-posta adresi
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
} 