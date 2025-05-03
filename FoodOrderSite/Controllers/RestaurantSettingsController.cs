using Microsoft.AspNetCore.Mvc;

public class RestaurantSettingsController : Controller
{
    // GET: /RestaurantSettings
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // POST: /RestaurantSettings
    [HttpPost]
    public IActionResult Index(IFormCollection form)
    {
        // Burada form verisini alıp işlemler yapabilirsiniz
        // Ama şu anda sadece görsellik için boş bırakabilirsiniz
        return View();
    }
}