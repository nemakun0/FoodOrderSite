using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Linq;
using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;

namespace FoodOrderSite.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : BaseAdminController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public AdminController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Settings()
        {
            var imagesPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            var images = Directory.GetFiles(imagesPath)
                .Select(Path.GetFileName)
                .Where(f => f.EndsWith(".jpg") || f.EndsWith(".png") || f.EndsWith(".jpeg"))
                .ToList();

            var currentBackground = HttpContext.Session.GetString("Background") ?? "";

            var model = new AdminSettingsViewModel
            {
                AvailableImages = images,
                CurrentBackground = currentBackground
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveSettings([FromBody] SettingsModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Geçersiz veri" });
            }

            var backgroundImage = string.IsNullOrEmpty(model.BackgroundImage) ? "" : model.BackgroundImage;
            HttpContext.Session.SetString("Background", backgroundImage);

            return Json(new { success = true, message = "Ayarlar başarıyla kaydedildi" });
        }

        public class SettingsModel
        {
            public string BackgroundImage { get; set; }
        }
    }
}
