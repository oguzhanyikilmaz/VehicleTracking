using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;

namespace VehicleTracking.API.Services
{
    public interface ILocationBroadcastService
    {
        Task BroadcastVehicleLocationAsync(VehicleDto vehicle);
        Task BroadcastVehicleLocationsAsync(IEnumerable<VehicleDto> vehicles);
    }
} 