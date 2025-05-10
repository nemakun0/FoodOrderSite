using Microsoft.AspNetCore.Mvc;
using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderSite.Controllers
{
    public class RestaurantSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RestaurantSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Kullanıcı ID'sini session'dan al
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Kullanıcıya ait restoran bilgilerini al
            var restaurant = await _context.RestaurantTables
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (restaurant == null)
            {
                return RedirectToAction("Index", "ManageRestaurant");
            }

            // Kullanıcı bilgilerini al
            var user = await _context.UserTables.FindAsync(userId);

            // Restoran çalışma saatlerini al
            var schedules = await _context.Schedules
                .Where(s => s.RestaurantId == restaurant.RestaurantId)
                .ToListAsync();

            // ViewModel oluştur
            var viewModel = new RestaurantSettingsViewModel
            {
                RestaurantId = restaurant.RestaurantId,
                RestaurantName = restaurant.RestaurantName,
                RestaurantType = restaurant.RestaurantType,
                Address = restaurant.Address,
                Description = restaurant.Description,
                City = restaurant.City,
                District = restaurant.District,
                Phone = user.Phone,
                Email = user.Email,
            };

            // Haftalık çalışma saatlerini oluştur
            string[] weekDays = { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi", "Pazar" };
            foreach (var day in weekDays)
            {
                var schedule = schedules.FirstOrDefault(s => s.DayOfWeek == day);
                
                if (schedule != null)
                {
                    viewModel.ScheduleItems.Add(new ScheduleViewModel
                    {
                        ScheduleId = schedule.ScheduleId,
                        DayOfWeek = day,
                        OpeningTime = schedule.OpeningTime,
                        ClosingTime = schedule.ClosingTime
                    });
                }
                else
                {
                    // Varsayılan saatler
                    viewModel.ScheduleItems.Add(new ScheduleViewModel
                    {
                        DayOfWeek = day,
                        OpeningTime = new TimeSpan(9, 0, 0),
                        ClosingTime = new TimeSpan(22, 0, 0)
                    });
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RestaurantSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kullanıcı ID'sini session'dan al
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Restoran bilgilerini güncelle
            var restaurant = await _context.RestaurantTables
                .FirstOrDefaultAsync(r => r.RestaurantId == model.RestaurantId && r.UserId == userId);

            if (restaurant == null)
            {
                return RedirectToAction("Index", "ManageRestaurant");
            }

            restaurant.RestaurantName = model.RestaurantName;
            restaurant.RestaurantType = model.RestaurantType;
            restaurant.Address = model.Address;
            restaurant.Description = model.Description;
            restaurant.City = model.City;
            restaurant.District = model.District;

            // Kullanıcı bilgilerini güncelle (email, telefon)
            var user = await _context.UserTables.FindAsync(userId);
            if (user != null)
            {
                user.Email = model.Email;
                user.Phone = model.Phone;
            }

            // Çalışma saatlerini güncelle
            foreach (var scheduleItem in model.ScheduleItems)
            {
                // Mevcut çalışma saati var mı kontrol et
                var existingSchedule = await _context.Schedules
                    .FirstOrDefaultAsync(s => s.RestaurantId == model.RestaurantId && s.DayOfWeek == scheduleItem.DayOfWeek);

                if (existingSchedule != null)
                {
                    // Güncelle
                    existingSchedule.OpeningTime = scheduleItem.OpeningTime;
                    existingSchedule.ClosingTime = scheduleItem.ClosingTime;
                }
                else
                {
                    // Yeni ekle
                    _context.Schedules.Add(new ScheduleTable
                    {
                        RestaurantId = model.RestaurantId,
                        DayOfWeek = scheduleItem.DayOfWeek,
                        OpeningTime = scheduleItem.OpeningTime,
                        ClosingTime = scheduleItem.ClosingTime
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Restoran bilgileri başarıyla güncellendi.";
            
            return RedirectToAction(nameof(Index));
        }
    }
}
