namespace VehicleTracking.Application.DTOs
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public long ExpiresIn { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public UserDto User { get; set; } = new UserDto();
    }
} 