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
            var role = HttpContext.Session.GetString("Role");
            var username = HttpContext.Session.GetString("Username");

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Username = username;

            string connectionString = _config.GetConnectionString("DefaultConnection")!;
            AuctionDLL auctionDLL = new AuctionDLL(connectionString);
            var bids = auctionDLL.GetAllBids();

            return View(bids);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }
    }
}