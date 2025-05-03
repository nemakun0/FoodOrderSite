using Microsoft.AspNetCore.Mvc;

namespace FoodOrderSite.Controllers
{
    public class OrdersandCustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
