using FoodOrderSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodOrderSite.Models.ViewModels;


public class RestaurantSettingsController : Controller
{
    private readonly ApplicationDbContext _context;

    public RestaurantSettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        int userId = 1; // Gerçekte bu login olan kullanıcıdan alınmalı

        var restaurant = _context.RestaurantTables
            .Include(r => r.User)
            .FirstOrDefault(r => r.UserId == userId);

        var schedules = _context.Schedules
            .Where(s => s.RestaurantId == restaurant.RestaurantId)
            .ToList();

        var viewModel = new RestaurantSettingsViewModel
        {
            RestaurantName = restaurant.RestaurantName,
            Phone = restaurant.User.Phone,
            Email = restaurant.User.Email,
            City = restaurant.City,
            District = restaurant.District,
            Address = restaurant.Address,
            Description = restaurant.Description,
            Schedules = schedules.Select(s => new ScheduleDto
            {
                DayOfWeek = s.DayOfWeek,
                OpeningTime = s.OpeningTime,
                ClosingTime = s.ClosingTime
            }).ToList()
        };

        return View(viewModel);
    }
}
