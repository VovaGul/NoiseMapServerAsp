using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoiseMapServerAsp.Hubs
{
    public class ClientConnectionHub : Hub
    {
        public async Task OnMarkerUpdated(int markerId)
        {
            await Clients.Others.SendAsync("UpdateMarker", markerId);
        }

        public async Task OnMarkerDeleted(int markerId)
        {
            await Clients.Others.SendAsync("DeleteMarker", markerId);
        }

        public async Task OnMarkerAdded(int markerId)
        {
            await Clients.Others.SendAsync("AddMarker", markerId);
        }
    }
}
