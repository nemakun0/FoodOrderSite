using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FoodOrderSite.Models;
using FoodOrderSite.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FoodOrderSite.Controllers
{
    public class CartController : Controller
    {
        private const string SessionKey = "Cart";
        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Sepet sayfasını görüntüle
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // Sepete ürün ekle
        [HttpPost]
        public IActionResult Add(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
                return NotFound();

            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == id);
            if (item == null)
            {
                cart.Add(new CartItemViewModel
                {
                    ProductId = id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }
            else
            {
                item.Quantity++;
            }

            SaveCart(cart);
            return RedirectToAction("Index", "Home");
        }

        // Oturumdan sepeti al
        private List<CartItemViewModel> GetCart()
        {
            var session = HttpContext.Session;
            var data = session.GetString(SessionKey);
            return string.IsNullOrEmpty(data)
                ? new List<CartItemViewModel>()
                : JsonConvert.DeserializeObject<List<CartItemViewModel>>(data)!;
        }

        // Oturuma sepeti kaydet
        private void SaveCart(List<CartItemViewModel> cart)
        {
            var session = HttpContext.Session;
            session.SetString(SessionKey, JsonConvert.SerializeObject(cart));
        }
    }
}
