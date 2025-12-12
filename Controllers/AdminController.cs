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
            // Simple pattern - create DLL with new keyword as professor expects
            string connectionString = _config.GetConnectionString("DefaultConnection")!;
            AuctionDLL auctionDLL = new AuctionDLL(connectionString);
            
            var bids = auctionDLL.GetAllBids();
            return View(bids);
        }
    }
}