using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSite.Controllers
{
    public class ReviewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
