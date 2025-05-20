using Microsoft.AspNetCore.Mvc;
using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;
using FoodOrderSite.Helpers; // ✅ Helper importu eklendi
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting; // ✅ IWebHostEnvironment için gerekli

namespace FoodOrderSite.Controllers
{
    public class RestaurantSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env; // ✅ Environment değişkeni eklendi

        public RestaurantSettingsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var restaurant = await _context.RestaurantTables
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (restaurant == null)
            {
                return RedirectToAction("Index", "ManageRestaurant");
            }

            var user = await _context.UserTables.FindAsync(userId);

            var schedules = await _context.Schedules
                .Where(s => s.RestaurantId == restaurant.RestaurantId)
                .ToListAsync();

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
                Image = restaurant.Image // ✅ Var olan görseli göstermek için
            };

            string[] weekDays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
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

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

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

            // ✅ Görseli yükle ve kaydet
            string? uploadedPath = await FileUploadHelper.UploadImageAsync(model.ImageFile, "uploads/restaurant", _env);
            if (uploadedPath != null)
            {
                restaurant.Image = uploadedPath;
            }

            var user = await _context.UserTables.FindAsync(userId);
            if (user != null)
            {
                user.Email = model.Email;
                user.Phone = model.Phone;
            }

            foreach (var scheduleItem in model.ScheduleItems)
            {
                var existingSchedule = await _context.Schedules
                    .FirstOrDefaultAsync(s => s.RestaurantId == model.RestaurantId && s.DayOfWeek == scheduleItem.DayOfWeek);

                if (existingSchedule != null)
                {
                    existingSchedule.OpeningTime = scheduleItem.OpeningTime;
                    existingSchedule.ClosingTime = scheduleItem.ClosingTime;
                }
                else
                {
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
            TempData["RestaurantSuccessMessage"] = "Restoran bilgileri başarıyla güncellendi.";

            return RedirectToAction(nameof(Index));
        }
    }
}
