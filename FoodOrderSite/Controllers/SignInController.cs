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

        //[HttpGet]
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new SignInViewModel();

            // Kullanıcı oturum açmış mı? 
            var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Eğer kullanıcı oturum açmışsa, email bilgilerini cookie'den alabiliriz
            if (authResult.Succeeded)
            {
                var emailClaim = authResult.Principal?.FindFirst(ClaimTypes.Name)?.Value;

                if (!string.IsNullOrEmpty(emailClaim))
                {
                    model.Email = emailClaim; // Cookie'den email bilgisini alıyoruz
                    model.Password = ""; // Şifreyi güvenlik için boş bırakıyoruz
                }
            }

            // Eğer session'da daha önce saklanmış bir şifre varsa, onu getirebiliriz
            if (HttpContext.Session.GetString("UserPassword") != null)
            {
                model.Password = HttpContext.Session.GetString("UserPassword"); // Şifreyi session'dan alıyoruz
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.UserTables.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if (user != null)
            {
                var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // UserId ekleniyor
        new Claim("UserId", user.UserId.ToString()), // Alternatif olarak custom claim
        new Claim(ClaimTypes.Role, user.Role)
    };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe, // RememberMe checkbox'ına göre ayarla
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) // 7 gün boyunca hatırlansın
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                // Eğer "Beni Hatırla" kutusu işaretlenmişse, şifreyi session'a kaydedebiliriz
                if (model.RememberMe)
                {
                    HttpContext.Session.SetString("UserPassword", model.Password); // Şifreyi session'a kaydediyoruz
                }
                // Session, cookie vb. işlemleri burada yapabilirsin

                switch (user.Role?.ToLower()) // küçük harfe çevirip kontrol edelim
                {
                    case "seller":
                        return RedirectToAction("Index", "ManageRestaurant");
                    case "customer":
                        return RedirectToAction("Index", "CustomerHome"); // örnek müşteri sayfası
                    case "admin":
                        return RedirectToAction("Index", "AdminDashboard"); // örnek admin paneli
                    default:
                        ModelState.AddModelError("", "Rol tanımsız. Lütfen yöneticinizle iletişime geçin.");
                        return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "E-posta veya şifre yanlış.");
                return View(model);
            }
        }
    }
}
