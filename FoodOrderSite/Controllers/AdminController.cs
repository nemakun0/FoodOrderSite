using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace FoodOrderSite.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var salesViewModel = await GetAllRestaurantsSalesData();
            return View(salesViewModel);
        }

        private async Task<AllRestaurantsSalesViewModel> GetAllRestaurantsSalesData()
        {
            var viewModel = new AllRestaurantsSalesViewModel();
            
            // Tarih aralıklarını belirle
            var today = DateTime.Now.Date;
            int dayOfWeek = (int)today.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7;
            var startOfWeek = today.AddDays(-(dayOfWeek - 1));
            var endOfWeek = startOfWeek.AddDays(6);

            // Tüm restoranların toplam günlük satışları
            var timeSlots = new[] { "09:00", "12:00", "15:00", "18:00", "21:00" };
            var dailyOrders = await _db.Orders
                .Where(o => o.OrderDate.Date == today)
                .ToListAsync();

            foreach (var slot in timeSlots)
            {
                var hour = int.Parse(slot.Split(':')[0]);
                decimal amount = 0;

                if (dailyOrders.Any())
                {
                    var slotStart = new DateTime(today.Year, today.Month, today.Day, hour, 0, 0);
                    var slotEnd = (hour < 21) ? new DateTime(today.Year, today.Month, today.Day, hour + 3, 0, 0) : today.AddDays(1);
                    
                    amount = dailyOrders
                        .Where(o => o.OrderDate >= slotStart && o.OrderDate < slotEnd)
                        .Sum(o => o.TotalAmount);
                }

                viewModel.DailySales.Add(new AdminDailyTimeSlotSale { TimeSlot = slot, Amount = amount });
            }

            // Haftalık satışlar
            string[] dayNames = { "Pzt", "Sal", "Çar", "Per", "Cum", "Cmt", "Paz" };
            var weeklyOrders = await _db.Orders
                .Where(o => o.OrderDate.Date >= startOfWeek.Date && o.OrderDate.Date <= endOfWeek.Date)
                .ToListAsync();

            for (int i = 0; i < 7; i++)
            {
                var currentDay = startOfWeek.AddDays(i);
                decimal amount = 0;

                if (weeklyOrders.Any())
                {
                    amount = weeklyOrders
                        .Where(o => o.OrderDate.Date == currentDay.Date)
                        .Sum(o => o.TotalAmount);
                }

                viewModel.WeeklySales.Add(new AdminWeeklyDaySale { DayName = dayNames[i], Amount = amount });
            }

            // Aylık satışlar
            string[] monthNames = CultureInfo.GetCultureInfo("tr-TR").DateTimeFormat.MonthNames.Take(12).ToArray();
            
            for (int i = 0; i < 12; i++)
            {
                var startOfMonth = new DateTime(today.Year, i + 1, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                
                var monthlyAmount = await _db.Orders
                    .Where(o => o.OrderDate >= startOfMonth && o.OrderDate <= endOfMonth)
                    .SumAsync(o => o.TotalAmount);
                
                viewModel.MonthlySales.Add(new AdminMonthlyDaySale { MonthName = monthNames[i], Amount = monthlyAmount });
            }

            // Toplam gelir
            viewModel.TotalIncome = await _db.Orders.SumAsync(o => o.TotalAmount);
            viewModel.HasAnySalesData = viewModel.TotalIncome > 0;

            return viewModel;
        }
    }
}
