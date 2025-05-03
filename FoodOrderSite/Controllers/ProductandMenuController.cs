using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSite.Controllers
{
    public class ProductandMenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
