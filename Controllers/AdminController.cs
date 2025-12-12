using Microsoft.AspNetCore.Mvc;
using AuctionApp.Data;

namespace AuctionApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _config;

        public AdminController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            // Check if user is logged in as admin
            var role = TempData.Peek("Role")?.ToString();
            if (role != "Admin")
                return RedirectToAction("Login", "Home");

            string connectionString = _config.GetConnectionString("DefaultConnection")!;
            AuctionDLL auctionDLL = new AuctionDLL(connectionString);

            var bids = auctionDLL.GetAllBids();
            return View(bids);
        }
    }
}