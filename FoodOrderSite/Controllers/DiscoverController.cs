using FoodOrderSite.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodOrderSite.Controllers
{
    public class DiscoverController : Controller
    {
        private const int PageSize = 12;
        private List<Restaurant> _restaurants;

        public DiscoverController()
        {
            _restaurants = GenerateSampleData();
        }

        public IActionResult Index(string cuisineType = null, string sortBy = null,
                                string searchTerm = null, int page = 1)
        {
            var model = new DiscoverViewModel
            {
                SelectedCuisineType = cuisineType,
                SortBy = sortBy,
                SearchTerm = searchTerm,
                CurrentPage = page,
                CuisineTypes = _restaurants.Select(r => r.CuisineType).Distinct().ToList(),
                TotalRestaurants = _restaurants.Count,
                PageSize = PageSize
            };

            var filteredRestaurants = FilterRestaurants(_restaurants, cuisineType, searchTerm);
            filteredRestaurants = SortRestaurants(filteredRestaurants, sortBy);

            model.FilteredCount = filteredRestaurants.Count;
            model.TotalPages = (int)Math.Ceiling((double)filteredRestaurants.Count / PageSize);

            model.Restaurants = filteredRestaurants
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return View(model);
        }

        private List<Restaurant> FilterRestaurants(List<Restaurant> restaurants,
                                                 string cuisineType, string searchTerm)
        {
            var filtered = restaurants.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                filtered = filtered.Where(r =>
                    r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    r.CuisineType.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(cuisineType))
            {
                filtered = filtered.Where(r => r.CuisineType == cuisineType);
            }

            return filtered.ToList();
        }

        private List<Restaurant> SortRestaurants(List<Restaurant> restaurants, string sortBy)
        {
            return sortBy switch
            {
                "rating" => restaurants.OrderByDescending(r => r.Rating).ToList(),
                "deliveryTime" => restaurants.OrderBy(r => r.DeliveryTime).ToList(),
                "deliveryFee" => restaurants.OrderBy(r => r.DeliveryFee).ToList(),
                _ => restaurants.OrderByDescending(r => r.IsPromoted)
                               .ThenBy(r => r.Name)
                               .ToList()
            };
        }

        private List<Restaurant> GenerateSampleData()
        {
            return new List<Restaurant>
            {
                new Restaurant { Id = 1, Name = "Lezzet Durağı", CuisineType = "Türk",
                    DeliveryFee = 5.99m, MinOrderAmount = 20.00m, Rating = 4.5,
                    ImageUrl = "/images/rest1.jpg", DeliveryTime = 35, IsPromoted = true },
                new Restaurant { Id = 2, Name = "Pizza Palace", CuisineType = "İtalyan",
                    DeliveryFee = 7.99m, MinOrderAmount = 25.00m, Rating = 4.2,
                    ImageUrl = "/images/rest2.jpg", DeliveryTime = 40, IsPromoted = false },
                new Restaurant { Id = 3, Name = "Burger Town", CuisineType = "Fast Food",
                    DeliveryFee = 4.99m, MinOrderAmount = 15.00m, Rating = 4.0,
                    ImageUrl = "/images/rest3.jpg", DeliveryTime = 25, IsPromoted = true }
            };
        }
    }
}