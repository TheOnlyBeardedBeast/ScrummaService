using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ScrummaService.Models;

namespace ScrummaService.Hubs
{
    public class PokerHub : Hub
    {
        private static List<User> Connections = new List<User>();

        public override async Task OnConnectedAsync()
        {

            await base.OnConnectedAsync();
            await Clients.All.SendAsync("clientConnected");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            System.Console.WriteLine("Disconnected");
            User user = Connections.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);

            if (user != null)
            {
                Connections.Remove(user);
                await Clients.All.SendAsync("clientLeft", user.ConnectionId);
            }
        }

        public async Task OnJoin(string UserName)
        {
            User user = new User
            {
                ConnectionId = Context.ConnectionId,
                UserName = UserName,
                Role = RoleEnum.User,
            };

            if (Connections.Count > 0)
            {
                await Clients.Others.SendAsync("clientSyncRequest", Context.ConnectionId);
            }

            Connections.Add(user);

            await Clients.Caller.SendAsync("selfJoined", user);
            await Clients.Others.SendAsync("clientJoined", user);
        }

        public async Task OnVote(float num)
        {
            await Clients.Others.SendAsync("clientVoted", Context.ConnectionId, num);
        }

        public async Task OnSync(string connectionId, User user, int? seconds)
        {
            await Clients.Client(connectionId).SendAsync("clientSyncResponse", user, seconds);
        }

        public async Task OnTimerStart()
        {
            await Clients.All.SendAsync("clientTimerStart");
        }

        public async Task OnTimerStop()
        {
            await Clients.All.SendAsync("clientTimerStop");
        }

        public async Task OnClearVotes()
        {
            await Clients.All.SendAsync("clearVotes");
        }
    }
}