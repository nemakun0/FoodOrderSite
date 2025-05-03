using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static FoodOrderSite.Models.ViewModels.SellerRegisterViewModel;

namespace FoodOrderSite.Controllers
{
    public class SellerSignUpController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SellerSignUpController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
     
        public async Task<IActionResult> Index(SellerRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new UserTable
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    BirthDate = model.BirthDate,
                    Email = model.Email,
                    Phone = model.Phone,
                    Password = model.Password, // Not: Hashleme yapılmalı
                    Role = "seller"              // Rolü "customer" olarak ayarla
                };

                // Kullanıcıyı veritabanına ekle
                _context.UserTables.Add(user);
                await _context.SaveChangesAsync();

                var restaurant = new RestaurantTable
                {
                    UserId = user.UserId, // Bu değeri frontend'den veya login olan kullanıcıdan almalısın
                    RestaurantName = model.RestaurantName,
                    RestaurantType = model.RestaurantType,
                    Address = model.Address,
                    Description = model.Description,
                    City = model.City,
                    District = model.District
                };

                _context.RestaurantTables.Add(restaurant);
                await _context.SaveChangesAsync();

                // Geçici olarak başarı sayfasına yönlendirme
                TempData["Success"] = "Kayıt başarılı!";
                return RedirectToAction("Success");
            }

            // Hatalıysa form tekrar gösterilir
            return View(model);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
