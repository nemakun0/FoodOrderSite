using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSite.Controllers
{
    public class DiscoverController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
