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
            // await Clients.All.SendAsync("clientConnected");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            System.Console.WriteLine("Disconnected");
            User user = Connections.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);

            if (user != null)
            {
                await Clients.OthersInGroup(GetCurentGroup()).SendAsync("clientLeft", user.ConnectionId);
                await Groups.RemoveFromGroupAsync(user.ConnectionId, user.Group.ToString());
                Connections.Remove(user);
            }
        }

        public async Task OnJoin(User user)
        {
            user.ConnectionId = Context.ConnectionId;

            if (Connections.Count > 0)
            {
                await Clients.OthersInGroup(user.Group.ToString()).SendAsync("clientSyncRequest", Context.ConnectionId);
            }

            Connections.Add(user);
            await Groups.AddToGroupAsync(user.ConnectionId, user.Group.ToString());

            await Clients.Caller.SendAsync("selfJoined", user);
            await Clients.OthersInGroup(user.Group.ToString()).SendAsync("clientJoined", user);
        }

        public async Task OnVote(float num)
        {
            await Clients.OthersInGroup(GetCurentGroup()).SendAsync("clientVoted", Context.ConnectionId, num);
        }

        public async Task OnSync(string connectionId, User user, int? seconds)
        {
            await Clients.Client(connectionId).SendAsync("clientSyncResponse", user, seconds);
        }

        public async Task OnTimerStart()
        {
            await Clients.Group(GetCurentGroup()).SendAsync("clientTimerStart");
        }

        public async Task OnTimerStop()
        {
            await Clients.Group(GetCurentGroup()).SendAsync("clientTimerStop");
        }

        public async Task OnClearVotes()
        {
            await Clients.Group(GetCurentGroup()).SendAsync("clearVotes");
        }

        private string GetCurentGroup()
        {
            return Connections.FirstOrDefault(user => user.ConnectionId == Context.ConnectionId)?.Group.ToString();
        }
    }
}