using Microsoft.AspNetCore.Mvc;
using AuctionApp.Models;
using AuctionApp.Data;

namespace AuctionApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration config, ILogger<HomeController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("=== LOGIN GET - Page Loaded ===");
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            _logger.LogInformation("=== LOGIN POST - Form Submitted ===");

            // Log the raw form data
            _logger.LogInformation("=== RAW FORM DATA ===");
            foreach (var key in Request.Form.Keys)
            {
                _logger.LogInformation($"Form Key: '{key}' = '{Request.Form[key]}'");
            }

            // Log the model
            _logger.LogInformation($"Model is null: {model == null}");
            _logger.LogInformation($"Username received: '{model?.Username}'");
            _logger.LogInformation($"Password received: '{model?.Password}'");
            _logger.LogInformation($"ModelState.IsValid: {ModelState.IsValid}");

            // Log request details
            _logger.LogInformation($"Request Method: {Request.Method}");
            _logger.LogInformation($"Content-Type: {Request.ContentType}");
            _logger.LogInformation($"Has Form: {Request.HasFormContentType}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is INVALID. Errors:");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogWarning($"  - {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

            try
            {
                string connectionString = _config.GetConnectionString("DefaultConnection")!;
                _logger.LogInformation($"Connection string: {(string.IsNullOrEmpty(connectionString) ? "NULL/EMPTY" : "OK")}");

                AuctionDLL auctionDLL = new AuctionDLL(connectionString);
                _logger.LogInformation("AuctionDLL created successfully");

                var bidder = auctionDLL.LoginUser(model.Username, model.Password);
                _logger.LogInformation($"LoginUser returned: {(bidder == null ? "NULL" : "Valid Bidder")}");

                if (bidder != null)
                {
                    _logger.LogInformation($"Login SUCCESSFUL for user: {bidder.Username}, Role: {bidder.Role}, ID: {bidder.BidderID}");

                    TempData["BidderID"] = bidder.BidderID;
                    TempData["Username"] = bidder.Username;
                    TempData["Role"] = bidder.Role;

                    _logger.LogInformation($"TempData saved - BidderID: {TempData["BidderID"]}, Username: {TempData["Username"]}, Role: {TempData["Role"]}");

                    if (bidder.Role == "Admin")
                    {
                        _logger.LogInformation("Redirecting to Admin/Index");
                        return RedirectToAction("Index", "Admin");
                    }

                    _logger.LogInformation("Redirecting to Bidder/Index");
                    return RedirectToAction("Index", "Bidder");
                }

                _logger.LogWarning("Login FAILED - Invalid credentials");
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"EXCEPTION during login: {ex.GetType().Name}");
                _logger.LogError($"Message: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");

                ModelState.AddModelError("", $"Login error: {ex.Message}");
                return View(model);
            }
        }
    }
}