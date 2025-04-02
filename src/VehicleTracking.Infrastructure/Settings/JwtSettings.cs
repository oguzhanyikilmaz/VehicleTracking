namespace VehicleTracking.Infrastructure.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; } = 60;
        public int RefreshTokenDurationInDays { get; set; } = 7;
    }
} 