using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodOrderSite.Controllers
{
    
    public class SavedAddressController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SavedAddressController(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<IActionResult> SavedAddresses()
        //{
        //    // Kullanıcının kimliğini al
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null)
        //    {
        //        // Eğer kullanıcı giriş yapmamışsa giriş sayfasına yönlendir
        //        return RedirectToAction("Index", "SignIn");
        //    }

        //    int userId = int.Parse(userIdClaim.Value);

        //    // Sadece o kullanıcıya ait adresleri çek
        //    var addresses = await _context.CustomerDeliveryAdderss
        //        .Where(a => a.UserId == userId)
        //        .Select(a => new CustomerDeliveryAddressViewModel
        //        {
        //            AddressId = a.AddressId,
        //            Title = a.Title,
        //            AddressLine = a.AddressLine,
        //            City = a.City,
        //            District = a.District
        //        })
        //        .ToListAsync();

        //    return View(addresses);
        //}
        [HttpGet]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var address = await _context.CustomerDeliveryAdderss
                .Where(a => a.AddressId == id)
                .Select(a => new CustomerDeliveryAddressViewModel
                {
                    AddressId = a.AddressId,
                    Title = a.Title,
                    AddressLine = a.AddressLine,
                    City = a.City,
                    District = a.District
                })
                .FirstOrDefaultAsync();

            if (address == null)
                return NotFound();

            return Json(address);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAddress(CustomerDeliveryAddressViewModel model)
        {
            var address = await _context.CustomerDeliveryAdderss.FindAsync(model.AddressId);
            if (address == null) return NotFound();

            address.Title = model.Title;
            address.City = model.City;
            address.District = model.District;
            address.AddressLine = model.AddressLine;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddAddress(CustomerDeliveryAddressViewModel model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Index", "SignIn");
            }

            int userId = int.Parse(userIdClaim.Value);

            var newAddress = new CustomerDeliveryAdderss
            {
                UserId = userId,
                Title = model.Title,
                AddressLine = model.AddressLine,
                City = model.City,
                District = model.District
            };

            _context.CustomerDeliveryAdderss.Add(newAddress);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var address = await _context.CustomerDeliveryAdderss.FindAsync(id);
            if (address != null)
            {
                _context.CustomerDeliveryAdderss.Remove(address);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Index", "SignIn");
            }

            int userId = int.Parse(userIdClaim.Value);

            var addresses = await _context.CustomerDeliveryAdderss
                .Where(a => a.UserId == userId)
                .Select(a => new CustomerDeliveryAddressViewModel
                {
                    AddressId = a.AddressId,
                    Title = a.Title,
                    AddressLine = a.AddressLine,
                    City = a.City,
                    District = a.District
                })
                .ToListAsync();

            return View(addresses); // ✅ artık model gönderiliyor
        }
    }
}
