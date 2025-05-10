using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSite.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
