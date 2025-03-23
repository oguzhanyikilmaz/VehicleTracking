namespace VehicleTracking.Infrastructure.TCP.Models
{
    public class TcpLocationData
    {
        public string DeviceId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public string RawData { get; set; }
    }
} 