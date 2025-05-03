using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSite.Controllers
{
    public class CustomerSignUpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
