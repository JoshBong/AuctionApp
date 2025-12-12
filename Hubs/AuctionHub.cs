using Microsoft.AspNetCore.SignalR;

namespace AuctionApp.Hubs
{
    public class AuctionHub : Hub
    {
        public async Task JoinAuctionRoom()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AuctionRoom");
            await Clients.Group("AuctionRoom").SendAsync("UserJoined", Context.User?.Identity?.Name ?? "Anonymous");
        }

        public async Task LeaveAuctionRoom()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AuctionRoom");
            await Clients.Group("AuctionRoom").SendAsync("UserLeft", Context.User?.Identity?.Name ?? "Anonymous");
        }

        public async Task NotifyBidPlaced(int itemId, decimal newPrice, string bidderName)
        {
            await Clients.Group("AuctionRoom").SendAsync("BidUpdated", itemId, newPrice, bidderName);
        }

        public override async Task OnConnectedAsync()
        {
            await JoinAuctionRoom();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await LeaveAuctionRoom();
            await base.OnDisconnectedAsync(exception);
        }
    }
}