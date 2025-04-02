namespace VehicleTracking.Application.DTOs
{
    public class DeviceConnectionUpdateDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
    }
} 