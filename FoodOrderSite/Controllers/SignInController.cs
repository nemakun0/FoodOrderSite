using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            // Eğer kullanıcı zaten giriş yapmışsa, rolüne göre yönlendir
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("admin"))
                    return RedirectToAction("Index", "Admin");
                else if (User.IsInRole("seller"))
                    return RedirectToAction("Index", "ManageRestaurant");
                else if (User.IsInRole("customer"))
                    return RedirectToAction("Index", "Discover");
            }
            return View(new SignInViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.UserTables.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null)
            {
                TempData["Error"] = "E-posta veya şifre hatalı!";
                return View(model);
            }

            if (!user.Status)
            {
                TempData["Error"] = "Hesabınız askıya alınmıştır. Lütfen yönetici ile iletişime geçin.";
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToLower())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                AllowRefresh = true
            };

            if (model.RememberMe)
            {
                authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            // Kullanıcıyı rolüne göre yönlendir
            switch (user.Role?.ToLower())
            {
                case "admin":
                    return RedirectToAction("Index", "Admin");
                case "seller":
                    return RedirectToAction("Index", "ManageRestaurant");
                case "customer":
                    return RedirectToAction("Index", "Discover");
                default:
                    ModelState.AddModelError("", "Rol tanımsız. Lütfen yöneticinizle iletişime geçin.");
                    return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            // Authentication cookie'sini temizle
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Session'ı temizle
            HttpContext.Session.Clear();
            
            // Tüm cookie'leri temizle
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
