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
        private readonly ILogger<BidderController> _logger;

        public BidderController(IConfiguration config, IHubContext<AuctionHub> hubContext, ILogger<BidderController> logger)
        {
            _config = config;
            _hubContext = hubContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("=== BIDDER INDEX - Page Requested ===");

            // Check if user is logged in
            var username = TempData.Peek("Username")?.ToString();
            var bidderId = TempData.Peek("BidderID");
            var role = TempData.Peek("Role")?.ToString();

            _logger.LogInformation($"TempData Username: '{username}'");
            _logger.LogInformation($"TempData BidderID: '{bidderId}'");
            _logger.LogInformation($"TempData Role: '{role}'");

            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("No username in TempData - redirecting to Login");
                return RedirectToAction("Login", "Home");
            }

            try
            {
                string connectionString = _config.GetConnectionString("DefaultConnection")!;
                AuctionDLL auctionDLL = new AuctionDLL(connectionString);

                var items = auctionDLL.GetAuctionItems();
                _logger.LogInformation($"Loaded {items?.Count ?? 0} auction items");

                return View(items);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading auction items: {ex.Message}");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> PlaceBid(int itemId, decimal amount)
        {
            _logger.LogInformation($"=== PLACE BID - ItemID: {itemId}, Amount: {amount} ===");

            var bidderId = TempData.Peek("BidderID") as int?;
            var username = TempData.Peek("Username")?.ToString();

            _logger.LogInformation($"BidderID: {bidderId}, Username: '{username}'");

            if (bidderId == null || string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("User not logged in - redirecting to Login");
                return RedirectToAction("Login", "Home");
            }

            try
            {
                string connectionString = _config.GetConnectionString("DefaultConnection")!;
                AuctionDLL auctionDLL = new AuctionDLL(connectionString);

                bool success = auctionDLL.PlaceBid(itemId, bidderId.Value, amount);
                _logger.LogInformation($"PlaceBid result: {success}");

                if (success)
                {
                    await _hubContext.Clients.Group("AuctionRoom").SendAsync("BidUpdated", itemId, amount, username);
                    TempData["Message"] = "Bid placed successfully!";
                    _logger.LogInformation("Bid successful, SignalR notification sent");
                }
                else
                {
                    TempData["Message"] = "Bid too low.";
                    _logger.LogWarning("Bid failed - amount too low");
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error placing bid: {ex.Message}");
                throw;
            }
        }
    }
}