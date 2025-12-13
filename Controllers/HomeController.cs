using Microsoft.AspNetCore.Mvc;
using AuctionApp.Models;
using AuctionApp.Data;

namespace AuctionApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string connectionString = _config.GetConnectionString("DefaultConnection")!;
            AuctionDLL auctionDLL = new AuctionDLL(connectionString);
            var bidder = auctionDLL.LoginUser(model.Username, model.Password);

            if (bidder != null)
            {
                HttpContext.Session.SetInt32("BidderID", bidder.BidderID);
                HttpContext.Session.SetString("Username", bidder.Username);
                HttpContext.Session.SetString("Role", bidder.Role);

                if (bidder.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Index", "Bidder");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }
    }
}