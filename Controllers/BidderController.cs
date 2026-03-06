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
            var username = HttpContext.Session.GetString("Username");
            var bidderId = HttpContext.Session.GetInt32("BidderID");

            if (string.IsNullOrEmpty(username) || !bidderId.HasValue)
                return RedirectToAction("Login", "Home");

            ViewBag.Username = username;

            string connectionString = _config.GetConnectionString("DefaultConnection")!;
            var auctionDLL = new AuctionDLL(connectionString);
            var items = auctionDLL.GetAuctionItems();

            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceBid(int itemId, decimal amount)
        {
            var bidderId = HttpContext.Session.GetInt32("BidderID");
            var username = HttpContext.Session.GetString("Username");

            if (!bidderId.HasValue || string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Home");

            string connectionString = _config.GetConnectionString("DefaultConnection")!;
            var auctionDLL = new AuctionDLL(connectionString);

            bool success = auctionDLL.PlaceBid(itemId, bidderId.Value, amount);

            if (success)
            {
                await _hubContext.Clients.Group("AuctionRoom").SendAsync("BidUpdated", itemId, amount, username);
                TempData["Message"] = "Bid placed successfully!";
            }
            else
            {
                TempData["Error"] = "Your bid wasn't high enough. Try going higher!";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }
    }
}
