using Microsoft.AspNetCore.SignalR;
using WebSellingCosmetics.Models;
using WebSellingCosmetics.Models.ViewModel;

namespace SR.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(MessageModel messageModel)
        {
            await Clients.All.SendAsync("ReceiveMessage", messageModel);
        }
        public async Task SendMessageToCall(string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }
        public async Task SendMessageToUser(string connectionId, string message)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }
        public override async Task  OnConnectedAsync()
        {
            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(ex);
        }
    }
}
