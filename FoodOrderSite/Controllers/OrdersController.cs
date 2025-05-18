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
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrdersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OrderHistory(string statusFilter = null)
        {
            // Get the current user ID
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "SignIn");
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Build the query based on filters
            IQueryable<Order> query = _db.Orders.Include(o => o.Restaurant).Where(o => o.UserId == userId);
            
            // Apply status filter if provided
            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(o => o.OrderStatus == statusFilter);
            }

            // Execute the query and order the results
            var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();

            // Create the view model
            var viewModel = new CustomerOrderHistoryViewModel
            {
                Orders = new List<CustomerOrderViewModel>(),
                CurrentFilter = statusFilter
            };

            // For each order, get the order items and check if review exists
            foreach (var order in orders)
            {
                var orderItems = await _db.OrderItems
                    .Where(oi => oi.OrderId == order.Id)
                    .ToListAsync();

                // Check if this order already has a review
                bool hasReview = await _db.ReviewTable.AnyAsync(r => r.OrderId == order.Id);

                viewModel.Orders.Add(new CustomerOrderViewModel
                {
                    Order = order,
                    OrderItems = orderItems,
                    HasReview = hasReview
                });
            }

            return View(viewModel);
        }

        // GET action to show review form
        public async Task<IActionResult> WriteReview(Guid orderId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "SignIn");
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Get the order with restaurant details
            var order = await _db.Orders
                .Include(o => o.Restaurant)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            // Check if the order is delivered - only delivered orders can be reviewed
            if (order.OrderStatus != "Delivered")
            {
                TempData["ErrorMessage"] = "Sadece teslim edilmiş siparişler için değerlendirme yapabilirsiniz.";
                return RedirectToAction("OrderHistory");
            }

            // Check if the order already has a review
            var existingReview = await _db.ReviewTable.FirstOrDefaultAsync(r => r.OrderId == orderId);
            if (existingReview != null)
            {
                TempData["ErrorMessage"] = "Bu sipariş için zaten bir değerlendirme yapmışsınız.";
                return RedirectToAction("OrderHistory");
            }

            // Create the view model for the review form
            var viewModel = new CreateReviewViewModel
            {
                OrderId = order.Id,
                RestaurantId = order.RestaurantId,
                RestaurantName = order.Restaurant?.RestaurantName ?? "Bilinmeyen Restoran",
                OrderDate = order.OrderDate,
                // Default values for ratings
                TasteRating = 5,
                ServiceRating = 5,
                DeliveryRating = 5,
                OverallRating = 5
            };

            return View(viewModel);
        }

        // POST action to save the review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WriteReview(CreateReviewViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "SignIn");
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Validate the order exists and belongs to this user
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == model.OrderId && o.UserId == userId);
            if (order == null)
            {
                return NotFound();
            }

            // Check if the order is delivered
            if (order.OrderStatus != "Delivered")
            {
                TempData["ErrorMessage"] = "Sadece teslim edilmiş siparişler için değerlendirme yapabilirsiniz.";
                return RedirectToAction("OrderHistory");
            }

            // Check if the order already has a review
            var existingReview = await _db.ReviewTable.FirstOrDefaultAsync(r => r.OrderId == model.OrderId);
            if (existingReview != null)
            {
                TempData["ErrorMessage"] = "Bu sipariş için zaten bir değerlendirme yapmışsınız.";
                return RedirectToAction("OrderHistory");
            }

            if (ModelState.IsValid)
            {
                // Create and save the review
                var review = new ReviewTable
                {
                    OrderId = model.OrderId,
                    RestaurantId = model.RestaurantId,
                    UserId = userId,
                    TasteRating = model.TasteRating,
                    ServiceRating = model.ServiceRating,
                    DeliveryRating = model.DeliveryRating,
                    OverallRating = model.OverallRating,
                    Comment = model.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                _db.ReviewTable.Add(review);
                await _db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Değerlendirmeniz için teşekkür ederiz!";
                return RedirectToAction("OrderHistory", new { statusFilter = "Delivered" });
            }

            // If we got this far, something failed; redisplay form
            return View(model);
        }
    }
} 