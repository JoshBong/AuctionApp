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

            // Get user info from SESSION
            var username = HttpContext.Session.GetString("Username");
            var bidderId = HttpContext.Session.GetInt32("BidderID");

            _logger.LogInformation($"Session Username: '{username}'");
            _logger.LogInformation($"Session BidderID: '{bidderId}'");

            if (string.IsNullOrEmpty(username) || !bidderId.HasValue)
            {
                _logger.LogWarning("No session data - redirecting to Login");
                return RedirectToAction("Login", "Home");
            }

            // Pass username to view using ViewBag
            ViewBag.Username = username;

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

            var bidderId = HttpContext.Session.GetInt32("BidderID");
            var username = HttpContext.Session.GetString("Username");

            _logger.LogInformation($"BidderID: {bidderId}, Username: '{username}'");

            if (!bidderId.HasValue || string.IsNullOrEmpty(username))
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

        [HttpPost]
        public IActionResult Logout()
        {
            _logger.LogInformation("Logout successfully");
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }
    }
}