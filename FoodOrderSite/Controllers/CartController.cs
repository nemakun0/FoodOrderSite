using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FoodOrderSite.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FoodOrderSite.Models.ViewModels;

namespace FoodOrderSite.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private const string CartSessionKey = "Cart"; // Standardized Key

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Consolidated method to get cart items from session
        private List<CartItemViewModel> GetCartItemsFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItemViewModel>();
            }
            try
            {
                // Explicitly deserialize to the correct ViewModel namespace
                return JsonConvert.DeserializeObject<List<FoodOrderSite.Models.ViewModels.CartItemViewModel>>(cartJson) ?? new List<CartItemViewModel>();
            }
            catch (JsonSerializationException)
            {
                HttpContext.Session.Remove(CartSessionKey);
                return new List<CartItemViewModel>();
            }
        }

        // Consolidated method to save cart items to session
        private void SaveCartItemsToSession(List<CartItemViewModel> cartItems)
        {
            var cartJson = JsonConvert.SerializeObject(cartItems);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }

        // Action to display the cart page using CartPageViewModel
        public IActionResult Index()
        {
            var cartItems = GetCartItemsFromSession();
            var viewModel = new CartPageViewModel(); // Using the detailed CartPageViewModel

            if (cartItems.Any())
            {
                viewModel.RestaurantGroups = cartItems
                    .GroupBy(item => new { item.RestaurantId, item.RestaurantName })
                    .Select(group => new RestaurantCartGroupViewModel
                    {
                        RestaurantId = group.Key.RestaurantId,
                        RestaurantName = group.Key.RestaurantName,
                        Items = group.ToList()
                    }).ToList();
            }
            return View(viewModel); // This should point to Views/Cart/Index.cshtml that expects CartPageViewModel
        }

        // AddItem correctly uses FoodItemId and the detailed CartItemViewModel
        public async Task<IActionResult> AddItem(int foodItemId, int quantity = 1)
        {
            if (foodItemId == 0 || quantity <= 0)
            {
                return BadRequest("Invalid item or quantity.");
            }

            var foodItem = await _db.FoodItemTables.Include(fi => fi.Restaurant)
                                    .FirstOrDefaultAsync(fi => fi.FoodItemId == foodItemId && fi.IsAvailable && !fi.IsDeleted);

            if (foodItem == null || foodItem.Restaurant == null || !foodItem.Restaurant.IsActive)
            {
                TempData["ErrorMessage"] = "This item is no longer available or the restaurant is inactive.";
                return RedirectToAction("Index", "Menu", new { restaurantId = foodItem?.RestaurantId ?? 0 });
            }

            var cart = GetCartItemsFromSession();
            var cartItem = cart.FirstOrDefault(item => item.FoodItemId == foodItemId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                // Using the constructor from FoodOrderSite.ViewModels.CartItemViewModel
                cart.Add(new FoodOrderSite.Models.ViewModels.CartItemViewModel(foodItem, foodItem.Restaurant, quantity));
            }

            SaveCartItemsToSession(cart);
            TempData["SuccessMessage"] = $"{foodItem.Name} added to cart.";
            return RedirectToAction("Index", "Menu", new { restaurantId = foodItem.RestaurantId });
        }

        public IActionResult RemoveItem(int foodItemId)
        {
            if (foodItemId == 0) return BadRequest();
            var cart = GetCartItemsFromSession();
            var itemToRemove = cart.FirstOrDefault(item => item.FoodItemId == foodItemId);

            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCartItemsToSession(cart);
                TempData["SuccessMessage"] = "Item removed from cart.";
            }
            return RedirectToAction("Index"); // Redirects to the cart page (Cart/Index)
        }

        // Action to Increment Item Quantity
        [HttpPost]
        [ValidateAntiForgeryToken] // Good practice for POST actions changing data
        public IActionResult IncrementItem(int foodItemId)
        {
            if (foodItemId == 0) return BadRequest();

            var cart = GetCartItemsFromSession();
            var cartItem = cart.FirstOrDefault(item => item.FoodItemId == foodItemId);

            if (cartItem != null)
            {
                cartItem.Quantity++; // Increment quantity
                SaveCartItemsToSession(cart);
                // Optional: Add TempData message
            }
            else
            {
                TempData["ErrorMessage"] = "Item not found in cart.";
            }
            return RedirectToAction("Index");
        }

        // Action to Decrement Item Quantity (and remove if zero)
        [HttpPost]
        [ValidateAntiForgeryToken] // Good practice for POST actions changing data
        public IActionResult DecrementItem(int foodItemId)
        {
            if (foodItemId == 0) return BadRequest();

            var cart = GetCartItemsFromSession();
            var cartItem = cart.FirstOrDefault(item => item.FoodItemId == foodItemId);

            if (cartItem != null)
            {
                cartItem.Quantity--; // Decrement quantity

                if (cartItem.Quantity <= 0)
                {
                    // Remove item if quantity is zero or less
                    cart.Remove(cartItem);
                    TempData["SuccessMessage"] = "Item removed from cart.";
                }
                
                SaveCartItemsToSession(cart);
                // Optional: Add TempData message if quantity > 0
            }
            else
            {
                TempData["ErrorMessage"] = "Item not found in cart.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CartPageViewModel model)
        {
            var cartItems = GetCartItemsFromSession();
            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            if (model.PaymentMethod == "CreditCard")
            {
                if (string.IsNullOrWhiteSpace(model.CreditCard.CardNumber) || 
                    string.IsNullOrWhiteSpace(model.CreditCard.ExpiryMonth) || 
                    string.IsNullOrWhiteSpace(model.CreditCard.ExpiryYear) || 
                    string.IsNullOrWhiteSpace(model.CreditCard.Cvv) ||
                    string.IsNullOrWhiteSpace(model.CreditCard.CardHolderName))
                {
                    TempData["ErrorMessage"] = "Please fill in all credit card details.";
                    model.RestaurantGroups = cartItems
                        .GroupBy(item => new { item.RestaurantId, item.RestaurantName })
                        .Select(group => new RestaurantCartGroupViewModel
                        {
                            RestaurantId = group.Key.RestaurantId,
                            RestaurantName = group.Key.RestaurantName,
                            Items = group.ToList()
                        }).ToList();
                    return View("Index", model);
                }
                System.Diagnostics.Debug.WriteLine($"Processing credit card: {model.CreditCard.CardNumber}");
            }
            else if (model.PaymentMethod == "Cash")
            {
                 System.Diagnostics.Debug.WriteLine("Cash payment selected.");
            }
            else
            {
                TempData["ErrorMessage"] = "Please select a payment method.";
                 model.RestaurantGroups = cartItems
                        .GroupBy(item => new { item.RestaurantId, item.RestaurantName })
                        .Select(group => new RestaurantCartGroupViewModel
                        {
                            RestaurantId = group.Key.RestaurantId,
                            RestaurantName = group.Key.RestaurantName,
                            Items = group.ToList()
                        }).ToList();
                return View("Index", model); 
            }

            // TODO: Implement actual order creation logic here
            SaveCartItemsToSession(new List<CartItemViewModel>()); 
            TempData["SuccessMessage"] = "Order placed successfully! (Simulation)";
            return RedirectToAction("Index", "Home"); 
        }
    }
}
