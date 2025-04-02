using System;
using System.Collections.Generic;

namespace VehicleTracking.Application.DTOs
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = new UserDto();
        public List<string> Permissions { get; set; } = new List<string>();
    }
} 