using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSite.Controllers
{
    public class ManageRestaurantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
