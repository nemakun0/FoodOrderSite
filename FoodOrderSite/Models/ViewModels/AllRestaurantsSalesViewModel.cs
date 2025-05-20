using System.Collections.Generic;

namespace FoodOrderSite.Models.ViewModels
{
    public class AllRestaurantsSalesViewModel
    {
        // Daily sales data
        public List<AdminDailyTimeSlotSale> DailySales { get; set; } = new List<AdminDailyTimeSlotSale>();
        
        // Weekly sales data
        public List<AdminWeeklyDaySale> WeeklySales { get; set; } = new List<AdminWeeklyDaySale>();
        
        // Monthly sales data
        public List<AdminMonthlyDaySale> MonthlySales { get; set; } = new List<AdminMonthlyDaySale>();
        
        // Financial metrics
        public decimal TotalIncome { get; set; }
        
        // Flag to indicate if there is any sales data to display
        public bool HasAnySalesData { get; set; }
    }

    public class AdminDailyTimeSlotSale
    {
        public string TimeSlot { get; set; }
        public decimal Amount { get; set; }
    }
    
    public class AdminWeeklyDaySale
    {
        public string DayName { get; set; }
        public decimal Amount { get; set; }
    }
    
    public class AdminMonthlyDaySale
    {
        public string MonthName { get; set; }
        public decimal Amount { get; set; }
    }
} 