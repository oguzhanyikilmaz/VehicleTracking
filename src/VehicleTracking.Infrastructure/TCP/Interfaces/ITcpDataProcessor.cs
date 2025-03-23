using System.Threading.Tasks;
using VehicleTracking.Infrastructure.TCP.Models;

namespace VehicleTracking.Infrastructure.TCP.Interfaces
{
    public interface ITcpDataProcessor
    {
        Task<TcpLocationData> ProcessDataAsync(string rawData);
    }
} 