using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderSite.Controllers
{
    public class DiscoverController : Controller
    {
        private const int PageSize = 8;
        private readonly ApplicationDbContext _db;
        private const string CartSessionKey = "Cart";

        public DiscoverController(ApplicationDbContext db)
        {
            _db = db;
        }

        private int GetCartItemCountFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson)) return 0;
            try
            {
                var items = JsonConvert.DeserializeObject<List<FoodOrderSite.Models.ViewModels.CartItemViewModel>>(cartJson);
                return items?.Sum(i => i.Quantity) ?? 0;
            }
            catch
            {
                HttpContext.Session.Remove(CartSessionKey);
                return 0;
            }
        }

        public async Task<IActionResult> Index(string district = null, string restaurantType = null, string sortBy = null,
                                string searchTerm = null, int page = 1)
        {
            var allRestaurantsQuery = _db.RestaurantTables.Where(r => r.IsActive).AsQueryable();
            var totalRestaurantCount = await allRestaurantsQuery.CountAsync();

            var allDistricts = await _db.RestaurantTables
                                  .Where(r => r.IsActive && r.District != null)
                                  .Select(r => r.District)
                                  .Distinct()
                                  .OrderBy(d => d)
                                  .ToListAsync();

            var allRestaurantTypes = await _db.RestaurantTables
                                     .Where(r => r.IsActive && r.RestaurantType != null)
                                     .Select(r => r.RestaurantType)
                                     .Distinct()
                                     .OrderBy(rt => rt)
                                     .ToListAsync();

            int cartItemCount = GetCartItemCountFromSession();

            var model = new DiscoverViewModel
            {
                SelectedDistrict = district,
                AllDistricts = allDistricts,
                SelectedRestaurantType = restaurantType,
                AllRestaurantTypes = allRestaurantTypes,
                SortBy = sortBy,
                SearchTerm = searchTerm,
                CurrentPage = page,
                TotalRestaurants = totalRestaurantCount,
                PageSize = PageSize,
                CartItemCount = cartItemCount
            };

            var filteredRestaurantTablesQuery = FilterRestaurantTables(allRestaurantsQuery, district, restaurantType, searchTerm);
            var filteredRestaurantTablesList = await filteredRestaurantTablesQuery.ToListAsync();
            var mappedRestaurants = MapRestaurantTablesToRestaurants(filteredRestaurantTablesList);
            var sortedRestaurants = SortRestaurants(mappedRestaurants, sortBy);

            model.FilteredCount = sortedRestaurants.Count;
            model.TotalPages = (int)Math.Ceiling((double)sortedRestaurants.Count / PageSize);
            model.Restaurants = sortedRestaurants
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return View(model);
        }

        private IQueryable<RestaurantTable> FilterRestaurantTables(IQueryable<RestaurantTable> restaurants,
                                                                 string district, string restaurantType, string searchTerm)
        {
            var filtered = restaurants;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                string lowerSearchTerm = searchTerm.ToLower();
                filtered = filtered.Where(r =>
                    (r.RestaurantName != null && r.RestaurantName.ToLower().Contains(lowerSearchTerm)) ||
                    (r.Description != null && r.Description.ToLower().Contains(lowerSearchTerm)) ||
                    (r.RestaurantType != null && r.RestaurantType.ToLower().Contains(lowerSearchTerm)));
            }

            if (!string.IsNullOrEmpty(district))
            {
                filtered = filtered.Where(r => r.District == district);
            }

            if (!string.IsNullOrEmpty(restaurantType))
            {
                filtered = filtered.Where(r => r.RestaurantType == restaurantType);
            }

            return filtered;
        }
        
        private List<Restaurant> SortRestaurants(List<Restaurant> restaurants, string sortBy)
        {
            return sortBy switch
            {
                "rating" => restaurants.OrderByDescending(r => r.Rating).ThenBy(r=>r.Name).ToList(),
                "deliveryTime" => restaurants.OrderBy(r => r.DeliveryTime).ThenBy(r=>r.Name).ToList(),
                "deliveryFee" => restaurants.OrderBy(r => r.DeliveryFee).ThenBy(r=>r.Name).ToList(),
                "minOrder" => restaurants.OrderBy(r => r.MinOrderAmount).ThenBy(r=>r.Name).ToList(),
                _ => restaurants.OrderBy(r => r.Name).ToList()
            };
        }

        private Restaurant MapRestaurantTableToRestaurant(RestaurantTable rt)
        {
            if (rt == null) return null;

            return new Restaurant
            {
                Id = rt.RestaurantId,
                Name = rt.RestaurantName,
                RestaurantType = rt.RestaurantType ?? "N/A",
                City = rt.City ?? "N/A",
                District = rt.District ?? "N/A",
                ShortDescription = rt.Description?.Length > 100 ? rt.Description.Substring(0, 97) + "..." : rt.Description ?? "No description available.",
                DeliveryFee = 5.99m,    
                MinOrderAmount = 20.00m, 
                Rating = CalculateRating(rt.RestaurantId), 
                ImageUrl = rt.Image,
                DeliveryTime = 30
            };
        }
        
        private double CalculateRating(int restaurantId)
        {
            return Math.Round(3.5 + (restaurantId % 15) / 10.0, 1);
        }

        private List<Restaurant> MapRestaurantTablesToRestaurants(List<RestaurantTable> rts)
        {
            return rts.Select(rt => MapRestaurantTableToRestaurant(rt)).ToList();
        }
    }
}