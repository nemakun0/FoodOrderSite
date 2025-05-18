using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json; // Added for session deserialization
using System.Collections.Generic; // Added for List<T>
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderSite.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _db;
        private const string CartSessionKey = "Cart"; // Match CartController key

        public MenuController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Helper to get cart count from session 
        private int GetCartItemCountFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson)) return 0;
            try
            {
                // Ensure correct namespace for CartItemViewModel
                var items = JsonConvert.DeserializeObject<List<FoodOrderSite.Models.ViewModels.CartItemViewModel>>(cartJson);
                return items?.Sum(i => i.Quantity) ?? 0;
            }
            catch
            {
                HttpContext.Session.Remove(CartSessionKey); // Clear potentially corrupted cart data
                return 0;
            }
        }

        // Helper to get cart items from session
        private List<CartItemViewModel> GetCartItemsFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItemViewModel>();
            }
            try
            {
                return JsonConvert.DeserializeObject<List<FoodOrderSite.Models.ViewModels.CartItemViewModel>>(cartJson) ?? new List<CartItemViewModel>();
            }
            catch
            {
                HttpContext.Session.Remove(CartSessionKey);
                return new List<CartItemViewModel>();
            }
        }

        public async Task<IActionResult> Index(int restaurantId)
        {
            if (restaurantId == 0)
            {
                return BadRequest("Restaurant ID is required.");
            }

            var restaurant = await _db.RestaurantTables
                                      .FirstOrDefaultAsync(r => r.RestaurantId == restaurantId && r.IsActive);

            if (restaurant == null)
            {
                // Potentially redirect to a more user-friendly error page or the Discover page
                return NotFound($"Active restaurant with ID {restaurantId} not found. Please try another restaurant.");
            }

            // Check if the cart has items from a different restaurant
            var cartItems = GetCartItemsFromSession();
            if (cartItems.Any() && cartItems.First().RestaurantId != restaurantId)
            {
                string existingRestaurantName = cartItems.First().RestaurantName;
                ViewBag.DifferentRestaurantWarning = $"Sepetinizde {existingRestaurantName} restoranından ürünler bulunmaktadır. Aynı anda yalnızca bir restorandan sipariş verebilirsiniz.";
            }

            var menuItems = await _db.FoodItemTables
                                     .Where(fi => fi.RestaurantId == restaurantId && fi.IsAvailable && !fi.IsDeleted)
                                     .OrderBy(fi => fi.Name) // Example ordering
                                     .ToListAsync();

            // Calculate cart count
            int cartItemCount = GetCartItemCountFromSession();

            // Optionally, group menu items by category if you have that relationship and want to display it.
            // For now, a flat list is implemented.

            var viewModel = new MenuPageViewModel
            {
                Restaurant = restaurant,
                MenuItems = menuItems,
                CartItemCount = cartItemCount // Pass count to view
            };

            return View(viewModel);
        }
    }
} 