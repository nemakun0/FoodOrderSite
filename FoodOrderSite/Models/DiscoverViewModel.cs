// Models/DiscoverViewModel.cs
using System.Collections.Generic;

namespace FoodOrderSite.Models
{
    public class DiscoverViewModel
    {
        public List<Restaurant> Restaurants { get; set; } // Keep as Restaurant for now, will map from RestaurantTable
        // public List<string> CuisineTypes { get; set; } // Removed
        // public string SelectedCuisineType { get; set; } // Removed
        public List<string> AllDistricts { get; set; }
        public string SelectedDistrict { get; set; }
        public List<string> AllRestaurantTypes { get; set; } // Added
        public string SelectedRestaurantType { get; set; } // Added
        
        // Category-related properties
        public List<CategoriesTable> AllCategories { get; set; }
        public string SelectedCategory { get; set; }
        
        public string SortBy { get; set; }
        public string SearchTerm { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12; // Default PageSize
        public int TotalRestaurants { get; set; } // Total count before filtering
        public int FilteredCount { get; set; } // Count after filtering
        public int CartItemCount { get; set; } // Added for cart icon
    }
}