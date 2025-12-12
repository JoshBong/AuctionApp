using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using AuctionApp.Data;
using AuctionApp.Hubs;

namespace AuctionApp.Controllers
{
    public class BidderController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHubContext<AuctionHub> _hubContext;

        public BidderController(IConfiguration config, IHubContext<AuctionHub> hubContext)
        {
            _config = config;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            // Simple pattern - create DLL with new keyword as professor expects
            string connectionString = _config.GetConnectionString("DefaultConnection")!;
            AuctionDLL auctionDLL = new AuctionDLL(connectionString);
            
            var items = auctionDLL.GetAuctionItems();
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceBid(int itemId, decimal amount)
        {
            if (TempData["BidderID"] == null)
                return RedirectToAction("Login", "Home");
                
            int bidderId = (int)TempData["BidderID"];
            string username = TempData["Username"]?.ToString() ?? "Unknown";
            TempData.Keep(); // keep data alive after redirect

            // Simple pattern - create DLL with new keyword as professor expects
            string connectionString = _config.GetConnectionString("DefaultConnection")!;
            AuctionDLL auctionDLL = new AuctionDLL(connectionString);
            
            bool success = auctionDLL.PlaceBid(itemId, bidderId, amount);

            if (success)
            {
                // Notify all connected clients via SignalR
                await _hubContext.Clients.Group("AuctionRoom").SendAsync("BidUpdated", itemId, amount, username);
                TempData["Message"] = "Bid placed successfully!";
            }
            else
            {
                TempData["Message"] = "Bid too low.";
            }

            return RedirectToAction("Index");
        }
    }
}