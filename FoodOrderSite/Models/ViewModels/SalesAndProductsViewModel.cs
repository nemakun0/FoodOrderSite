using System;
using System.Collections.Generic;
using FoodOrderSite.Models;

namespace FoodOrderSite.Models.ViewModels
{
    public class SalesAndProductsViewModel
    {
        // Restaurant information
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        
        // Daily sales data
        public List<TimeSlotSale> DailySales { get; set; } = new List<TimeSlotSale>();
        
        // Weekly sales data
        public List<DaySale> WeeklySales { get; set; } = new List<DaySale>();
        
        // Monthly sales data
        public List<MonthSale> MonthlySales { get; set; } = new List<MonthSale>();
        
        // Top selling products
        public List<TopProductViewModel> TopProducts { get; set; } = new List<TopProductViewModel>();
        
        // Top selling categories
        public List<TopCategoryViewModel> TopCategories { get; set; } = new List<TopCategoryViewModel>();
        
        // Financial metrics
        public decimal TotalIncome { get; set; }
        public double ProfitMargin { get; set; } = 25.0; // Default profit margin is 25%
    }
    
    public class TimeSlotSale
    {
        public string TimeSlot { get; set; }
        public decimal Amount { get; set; }
    }
    
    public class DaySale
    {
        public string DayName { get; set; }
        public decimal Amount { get; set; }
    }
    
    public class MonthSale
    {
        public string MonthName { get; set; }
        public decimal Amount { get; set; }
    }
    
    public class TopProductViewModel
    {
        public int FoodItemId { get; set; }
        public string ProductName { get; set; }
        public int SalesCount { get; set; }
    }
    
    public class TopCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SalesCount { get; set; }
    }
} 