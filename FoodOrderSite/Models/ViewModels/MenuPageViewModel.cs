using FoodOrderSite.Models;
using System.Collections.Generic;

namespace FoodOrderSite.Models.ViewModels
{
    public class MenuPageViewModel
    {
        public RestaurantTable Restaurant { get; set; }
        public List<FoodItemTable> MenuItems { get; set; }
        public int CartItemCount { get; set; } // Added for cart icon
        // Could add other properties if needed, e.g., categories for menu items if they are grouped.
        public string RestaurantImageUrl => 
        string.IsNullOrEmpty(Restaurant.Image) 
            ? "/images/default-restaurant.jpg" 
            : Restaurant.Image;
    }
} 