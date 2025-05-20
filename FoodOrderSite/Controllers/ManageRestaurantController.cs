using Microsoft.AspNetCore.Mvc;
using FoodOrderSite.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace FoodOrderSite.Controllers
{
    public class ManageRestaurantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageRestaurantController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Get the current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Index", "SignIn");
            }

            int userIdInt = int.Parse(userId);
            
            // Fetch restaurant data for the current user
            var restaurant = _context.RestaurantTables
                .FirstOrDefault(r => r.UserId == userIdInt);

            return View(restaurant);
        }
    }
}
