using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodOrderSite.Models;

namespace FoodOrderSite.Controllers
{
    public class RestaurantsController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public RestaurantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var restaurants = _context.RestaurantTables.ToList();
            return View(restaurants);
        }

        [HttpPost]
        public IActionResult ToggleStatus(int restaurantId, bool status)
        {
            try
            {
                var restaurant = _context.RestaurantTables.Find(restaurantId);
                if (restaurant == null)
                {
                    return Json(new { success = false, message = "Restoran bulunamadı" });
                }

                restaurant.IsActive = status;
                _context.Entry(restaurant).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new { success = true, message = "Restoran durumu güncellendi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }
    }
}