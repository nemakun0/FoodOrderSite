using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FoodOrderSite.Controllers
{
    public class BaseAdminController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            
            // Get background image from session
            var backgroundImage = HttpContext.Session.GetString("Background");
            ViewBag.CurrentBackground = backgroundImage;
        }
    }
} 