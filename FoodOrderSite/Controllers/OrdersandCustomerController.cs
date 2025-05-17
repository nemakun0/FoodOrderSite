using Microsoft.AspNetCore.Mvc;
using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System;
using System.Collections.Generic;

namespace FoodOrderSite.Controllers
{
    public class OrdersandCustomerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrdersandCustomerController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Get the current user ID
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "SignIn");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Get the restaurant associated with this user
            var restaurant = await _db.RestaurantTables
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (restaurant == null)
            {
                // This user doesn't have a restaurant
                return RedirectToAction("Index", "Home");
            }

            // Get all orders for this restaurant
            var orders = await _db.Orders
                .Where(o => o.RestaurantId == restaurant.RestaurantId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // Create the view model
            var viewModel = new RestaurantOrdersViewModel
            {
                RestaurantId = restaurant.RestaurantId,
                RestaurantName = restaurant.RestaurantName,
                Orders = new List<OrderWithDetails>(),
                LastUpdatedOrderId = TempData["LastUpdatedOrderId"]?.ToString()
            };

            // For each order, get the items and customer details
            foreach (var order in orders)
            {
                var orderItems = await _db.OrderItems
                    .Where(oi => oi.OrderId == order.Id)
                    .ToListAsync();

                var customer = await _db.UserTables
                    .FirstOrDefaultAsync(u => u.UserId == order.UserId);

                viewModel.Orders.Add(new OrderWithDetails
                {
                    Order = order,
                    OrderItems = orderItems,
                    Customer = customer
                });
            }

            return View(viewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, string status)
        {
            var order = await _db.Orders.FindAsync(orderId);
            
            if (order == null)
            {
                return NotFound();
            }
            
            // Make sure the order belongs to this restaurant
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var restaurant = await _db.RestaurantTables
                .FirstOrDefaultAsync(r => r.UserId == userId);
                
            if (restaurant == null || order.RestaurantId != restaurant.RestaurantId)
            {
                return Unauthorized();
            }
            
            // Update the status
            order.OrderStatus = status;
            await _db.SaveChangesAsync();
            
            // Store the orderId in TempData so the view can keep it expanded
            TempData["LastUpdatedOrderId"] = orderId.ToString();
            
            return RedirectToAction("Index");
        }
    }
}
