using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSite.Controllers
{
    public class SignInController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
