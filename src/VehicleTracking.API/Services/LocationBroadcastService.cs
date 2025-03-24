using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using VehicleTracking.API.Hubs;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Application.Services;

namespace VehicleTracking.API.Services
{
    public class LocationBroadcastService : ILocationBroadcastService
    {
        private readonly IHubContext<VehicleLocationHub> _hubContext;
        private readonly ILogger<LocationBroadcastService> _logger;

        public LocationBroadcastService(
            IHubContext<VehicleLocationHub> hubContext,
            ILogger<LocationBroadcastService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task BroadcastVehicleLocationAsync(VehicleDto vehicle)
        {
            try
            {
                var locationUpdate = new
                {
                    vehicle.Id,
                    vehicle.PlateNumber,
                    vehicle.Latitude,
                    vehicle.Longitude,
                    vehicle.Speed,
                    vehicle.LastUpdateTime
                };

                // İlgili araca abone olan istemcilere güncelleme gönder
                await _hubContext.Clients.Group(vehicle.Id)
                    .SendAsync("ReceiveLocationUpdate", locationUpdate);

                // Tüm araç güncellemelerine abone olan istemcilere güncelleme gönder
                await _hubContext.Clients.Group("AllVehicles")
                    .SendAsync("ReceiveLocationUpdate", locationUpdate);

                _logger.LogDebug("Broadcast location update for vehicle {VehicleId}", vehicle.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting location update for vehicle {VehicleId}", vehicle.Id);
            }
        }

        public async Task BroadcastVehicleLocationsAsync(IEnumerable<VehicleDto> vehicles)
        {
            try
            {
                var locationUpdates = new List<object>();
                foreach (var vehicle in vehicles)
                {
                    locationUpdates.Add(new
                    {
                        vehicle.Id,
                        vehicle.PlateNumber,
                        vehicle.Latitude,
                        vehicle.Longitude,
                        vehicle.Speed,
                        vehicle.LastUpdateTime
                    });

                    // İlgili araca abone olan istemcilere güncelleme gönder
                    await _hubContext.Clients.Group(vehicle.Id)
                        .SendAsync("ReceiveLocationUpdate", locationUpdates[locationUpdates.Count - 1]);
                }

                // Tüm araç güncellemelerine abone olan istemcilere toplu güncelleme gönder
                await _hubContext.Clients.Group("AllVehicles")
                    .SendAsync("ReceiveBatchLocationUpdate", locationUpdates);

                _logger.LogDebug("Broadcast batch location update for {Count} vehicles", locationUpdates.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting batch location updates");
            }
        }
    }
} 