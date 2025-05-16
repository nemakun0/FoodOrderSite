using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSite.Controllers
{
    public class SellerSignUpController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SellerSignUpController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new SellerRegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SellerRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Hatalıysa view tekrar gösterilir
            }

            // Email kontrolü
            if (_context.UserTables.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Bu email zaten kayıtlı.");
                return View(model);
            }

            // Telefon kontrolü
            if (_context.UserTables.Any(u => u.Phone == model.Phone))
            {
                ModelState.AddModelError("Phone", "Bu telefon numarası zaten kayıtlı.");
                return View(model);
            }

            var user = new UserTable
            {
                Name = model.Name,
                Surname = model.Surname,
                BirthDate = model.BirthDate,
                Email = model.Email,
                Phone = model.Phone,
                Password = model.Password, // NOT: Hash işlemi mutlaka eklenmeli!
                Role = "seller"
            };

            _context.UserTables.Add(user);
            await _context.SaveChangesAsync();

            var restaurant = new RestaurantTable
            {
                UserId = user.UserId,
                RestaurantName = model.RestaurantName,
                RestaurantType = model.RestaurantType,
                Address = model.Address,
                Description = model.Description,
                City = model.City,
                District = model.District
            };

            _context.RestaurantTables.Add(restaurant);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Kayıt başarılı!";
            return RedirectToAction("Index", "SignIn");
        }
    }
}

//using FoodOrderSite.Models;
//using FoodOrderSite.Models.ViewModels;
//using Microsoft.AspNetCore.Mvc;
//using System.Net;
//using static FoodOrderSite.Models.ViewModels.SellerRegisterViewModel;

//namespace FoodOrderSite.Controllers
//{
//    public class SellerSignUpController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public SellerSignUpController(ApplicationDbContext context)
//        {
//            _context = context;
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]

//        public async Task<IActionResult> Index(SellerRegisterViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                // ✅ Email kontrolü
//                if (_context.UserTables.Any(u => u.Email == model.Email))
//                {
//                    ModelState.AddModelError("Email", "Bu email zaten kayıtlı.");
//                    return View(model);
//                }

//                // ✅ Telefon kontrolü
//                if (_context.UserTables.Any(u => u.Phone == model.Phone))
//                {
//                    ModelState.AddModelError("Phone", "Bu telefon numarası zaten kayıtlı.");
//                    return View(model);
//                }
//                var user = new UserTable
//                {
//                    Name = model.Name,
//                    Surname = model.Surname,
//                    BirthDate = model.BirthDate,
//                    Email = model.Email,
//                    Phone = model.Phone,
//                    Password = model.Password, // Not: Hashleme yapılmalı
//                    Role = "seller"              // Rolü "customer" olarak ayarla
//                };

//                // Kullanıcıyı veritabanına ekle
//                _context.UserTables.Add(user);
//                await _context.SaveChangesAsync();

//                var restaurant = new RestaurantTable
//                {
//                    UserId = user.UserId, // Bu değeri frontend'den veya login olan kullanıcıdan almalısın
//                    RestaurantName = model.RestaurantName,
//                    RestaurantType = model.RestaurantType,
//                    Address = model.Address,
//                    Description = model.Description,
//                    City = model.City,
//                    District = model.District
//                };

//                _context.RestaurantTables.Add(restaurant);
//                await _context.SaveChangesAsync();

//                // Geçici olarak başarı sayfasına yönlendirme
//                TempData["Success"] = "Kayıt başarılı!";
//                return RedirectToAction("Index","SignIn");
//            }

//            // Hatalıysa form tekrar gösterilir
//            return View(model);
//        }
//        public IActionResult Index()
//        {
//            return View();
//        }
//    }
//}
