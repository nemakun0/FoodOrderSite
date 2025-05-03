using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FoodOrderSite.Models.ViewModels.SignInViewModel;

namespace FoodOrderSite.Controllers
{
    public class SignInController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SignInController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(SignInViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.UserTables.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if (user != null)
            {
                // Kullanıcı bulundu, giriş başarılı
                // İstersen Session / Cookie işlemleri yapabilirsin
                return RedirectToAction("Dashboard", "Home"); // örnek yönlendirme
            }
            else
            {
                ModelState.AddModelError("", "E-posta veya şifre yanlış.");
                return View(model);
            }
        }
    }
}
