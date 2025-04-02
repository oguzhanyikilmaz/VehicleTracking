using System;
using System.Collections.Generic;

namespace VehicleTracking.Application.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        
        // Kullanıcı seçenekleri
        public string Language { get; set; } = "tr";
        public string Theme { get; set; } = "light";
        
        // Tenant bilgisi
        public string TenantId { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        
        // Roller
        public List<string> RoleIds { get; set; } = new List<string>();
        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
        
        // Tam ad
        public string FullName => $"{FirstName} {LastName}";
    }
    
    // Kullanıcı oluşturma ve güncelleme için DTO
    public class UserCreateDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public List<string> RoleIds { get; set; } = new List<string>();
    }
    
    // Kullanıcı güncelleme için DTO
    public class UserUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string Language { get; set; } = "tr";
        public string Theme { get; set; } = "light";
    }
    
    // Şifre değiştirme için DTO
    public class ChangePasswordDto
    {
        public string Id { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
} 