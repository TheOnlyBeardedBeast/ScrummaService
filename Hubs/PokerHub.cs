using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrummaService.Hubs
{
    public class PokerHub : Hub
    {
        private static List<string> Connections = new List<string>();

        public override async Task OnConnectedAsync()
        {
            if (Connections.Count > 0)
            {
                await Clients.Client(Connections.First()).SendAsync("clientSync", Context.ConnectionId);
            }
            Connections.Add(Context.ConnectionId);

            await base.OnConnectedAsync();
            await Clients.All.SendAsync("clientConnected");
        }

        public async Task OnVote(float num)
        {
            await Clients.All.SendAsync("clientVoted", Context.ConnectionId,num);
        }
    }
}
