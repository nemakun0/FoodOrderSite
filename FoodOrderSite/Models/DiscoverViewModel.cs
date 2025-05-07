// Models/DiscoverViewModel.cs
using System.Collections.Generic;

namespace FoodOrderSite.Models
{
    public class DiscoverViewModel
    {
        public List<Restaurant> Restaurants { get; set; }
        public List<string> CuisineTypes { get; set; }
        public string SelectedCuisineType { get; set; }
        public string SortBy { get; set; }
        public string SearchTerm { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12;
        public int TotalRestaurants { get; set; }
        public int FilteredCount { get; set; }
    }
}