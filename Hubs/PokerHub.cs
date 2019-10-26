using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ScrummaService.Models;

namespace ScrummaService.Hubs {
    public class PokerHub : Hub {
        private static List<PokerConnection> Connections = new List<PokerConnection> ();

        public override async Task OnDisconnectedAsync (Exception exception) {
            var user = Connections.FirstOrDefault (c => c.ConnectionId == Context.ConnectionId);

            if (user != null) {
                await Clients.OthersInGroup (GetCurentGroup ()).SendAsync ("clientLeft", user.ConnectionId);
                await Groups.RemoveFromGroupAsync (user.ConnectionId, user.Group.ToString ());
                Connections.Remove (user);
            }
        }

        public async Task OnJoin (User user) {
            user.ConnectionId = Context.ConnectionId;

            if (Connections.Count > 0) {
                await Clients.OthersInGroup (user.Group.ToString ()).SendAsync ("clientSyncRequest", Context.ConnectionId);
            }

            Connections.Add (new PokerConnection {
                ConnectionId = user.ConnectionId,
                    Group = user.Group
            });
            await Groups.AddToGroupAsync (user.ConnectionId, user.Group.ToString ());

            await Clients.Caller.SendAsync ("selfJoined", user);
            await Clients.OthersInGroup (user.Group.ToString ()).SendAsync ("clientJoined", user);
        }

        public async Task OnVote (float num) {
            await Clients.OthersInGroup (GetCurentGroup ()).SendAsync ("clientVoted", Context.ConnectionId, num);
        }

        public async Task OnSync (string connectionId, User user, int? seconds) {
            await Clients.Client (connectionId).SendAsync ("clientSyncResponse", user, seconds);
        }

        public async Task OnTimerStart () {
            await Clients.Group (GetCurentGroup ()).SendAsync ("clientTimerStart");
        }

        public async Task OnTimerStop () {
            await Clients.Group (GetCurentGroup ()).SendAsync ("clientTimerStop");
        }

        public async Task OnClearVotes () {
            await Clients.Group (GetCurentGroup ()).SendAsync ("clearVotes");
        }

        public async Task OnSyncTitle (string title) {
            await Clients.OthersInGroup (GetCurentGroup ()).SendAsync ("syncTitle", title);
        }

        public async Task OnAddToHistory (HistoryItem historyItem) {
            await Clients.Group (GetCurentGroup ()).SendAsync ("addToHistory", historyItem);
        }

        public async Task OnSyncHistoryRequest (string connectionId) {
            Console.WriteLine ("OnSyncHistoryRequest " + connectionId);
            await Clients.Client (connectionId).SendAsync ("syncHistoryRequest", Context.ConnectionId);
        }

        public async Task OnSyncHistoryResponse (string connectionId, List<HistoryItem> history, string title) {
            await Clients.Client (connectionId).SendAsync ("syncHistoryResponse", history, title);
        }

        public async Task OnPause ()
        {
            await Clients.Group(GetCurentGroup()).SendAsync("pauseRequest");
        }

        private string GetCurentGroup () {
            return Connections.FirstOrDefault (user => user.ConnectionId == Context.ConnectionId)?.Group.ToString ();
        }
    }
}