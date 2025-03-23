using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VehicleTracking.Application.DTOs;

namespace VehicleTracking.API.Hubs
{
    public class VehicleLocationHub : Hub
    {
        public async Task SubscribeToVehicleUpdates(string vehicleId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, vehicleId);
        }

        public async Task UnsubscribeFromVehicleUpdates(string vehicleId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, vehicleId);
        }

        public async Task SubscribeToAllVehicleUpdates()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AllVehicles");
        }

        public async Task UnsubscribeFromAllVehicleUpdates()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AllVehicles");
        }
    }
} 