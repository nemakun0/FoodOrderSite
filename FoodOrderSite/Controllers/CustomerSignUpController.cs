using Microsoft.AspNetCore.Mvc;
using FoodOrderSite.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using static FoodOrderSite.Models.ViewModels.CustomerRegisterViewModel;

namespace FoodOrderSite.Controllers
{
    public class CustomerSignUpController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerSignUpController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // ✅ Email kontrolü
                if (_context.UserTables.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Bu email zaten kayıtlı.");
                    return View(model);
                }

                // ✅ Telefon kontrolü
                if (_context.UserTables.Any(u => u.Phone == model.Phone))
                {
                    ModelState.AddModelError("Phone", "Bu telefon numarası zaten kayıtlı.");
                    return View(model);
                }
                // 1. UserTable modelini oluştur
                var user = new UserTable
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    BirthDate = model.BirthDate,
                    Email = model.Email,
                    Phone = model.Phone,
                    Password = model.Password, // Not: Hashleme yapılmalı
                    Role = "customer"              // Rolü "customer" olarak ayarla
                };

                // Kullanıcıyı veritabanına ekle
                _context.UserTables.Add(user);
                await _context.SaveChangesAsync();

                // 2. CustomerDeliveryAddress modelini oluştur
                var address = new CustomerDeliveryAdderss
                {
                    UserId = user.UserId, // az önce oluşturduğumuz kullanıcının ID'si
                    AddressLine = model.AddressLine,
                    City = model.City,
                    District = model.District
                };

                // Adresi veritabanına ekle
                _context.CustomerDeliveryAdderss.Add(address);
                await _context.SaveChangesAsync();

                // Kayıt başarılıysa başka bir sayfaya yönlendirme yapabiliriz
                return RedirectToAction("Index", "SignIn"); // Kayıt başarılı ise bir başarı sayfasına yönlendirme
            }

            // Model geçerli değilse, tekrar formu göster
            return View(model);
        }

        // Telefon numarası validasyonu için remote action
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyPhone(string phone)
        {
            if (_context.UserTables.Any(u => u.Phone == phone))
            {
                return Json($"Bu telefon numarası zaten kayıtlı: {phone}");
            }
            return Json(true);
        }

        // Email validasyonu için remote action
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyEmail(string email)
        {
            if (_context.UserTables.Any(u => u.Email == email))
            {
                return Json($"Bu email adresi zaten kayıtlı: {email}");
            }
            return Json(true);
        }
    }
}
