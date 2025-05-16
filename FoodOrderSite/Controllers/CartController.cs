using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FoodOrderSite.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FoodOrderSite.Models.ViewModels;
using System;
using System.Diagnostics;

namespace FoodOrderSite.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private const string CartSessionKey = "Cart"; // Standardized Key

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Consolidated method to get cart items from session
        private List<CartItemViewModel> GetCartItemsFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItemViewModel>();
            }
            try
            {
                // Explicitly deserialize to the correct ViewModel namespace
                return JsonConvert.DeserializeObject<List<FoodOrderSite.Models.ViewModels.CartItemViewModel>>(cartJson) ?? new List<CartItemViewModel>();
            }
            catch (JsonSerializationException)
            {
                HttpContext.Session.Remove(CartSessionKey);
                return new List<CartItemViewModel>();
            }
        }

        // Consolidated method to save cart items to session
        private void SaveCartItemsToSession(List<CartItemViewModel> cartItems)
        {
            var cartJson = JsonConvert.SerializeObject(cartItems);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }

        // Action to display the cart page using CartPageViewModel
        public IActionResult Index()
        {
            var cartItems = GetCartItemsFromSession();
            var viewModel = new CartPageViewModel(); // Using the detailed CartPageViewModel

            if (cartItems.Any())
            {
                viewModel.RestaurantGroups = cartItems
                    .GroupBy(item => new { item.RestaurantId, item.RestaurantName })
                    .Select(group => new RestaurantCartGroupViewModel
                    {
                        RestaurantId = group.Key.RestaurantId,
                        RestaurantName = group.Key.RestaurantName,
                        Items = group.ToList()
                    }).ToList();
            }
            return View(viewModel); // This should point to Views/Cart/Index.cshtml that expects CartPageViewModel
        }

        // AddItem correctly uses FoodItemId and the detailed CartItemViewModel
        public async Task<IActionResult> AddItem(int foodItemId, int quantity = 1)
        {
            if (foodItemId == 0 || quantity <= 0)
            {
                return BadRequest("Invalid item or quantity.");
            }

            var foodItem = await _db.FoodItemTables.Include(fi => fi.Restaurant)
                                    .FirstOrDefaultAsync(fi => fi.FoodItemId == foodItemId && fi.IsAvailable && !fi.IsDeleted);

            if (foodItem == null || foodItem.Restaurant == null || !foodItem.Restaurant.IsActive)
            {
                TempData["ErrorMessage"] = "This item is no longer available or the restaurant is inactive.";
                return RedirectToAction("Index", "Menu", new { restaurantId = foodItem?.RestaurantId ?? 0 });
            }

            var cart = GetCartItemsFromSession();
            var cartItem = cart.FirstOrDefault(item => item.FoodItemId == foodItemId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                // Using the constructor from FoodOrderSite.ViewModels.CartItemViewModel
                cart.Add(new FoodOrderSite.Models.ViewModels.CartItemViewModel(foodItem, foodItem.Restaurant, quantity));
            }

            SaveCartItemsToSession(cart);
            TempData["SuccessMessage"] = $"{foodItem.Name} added to cart.";
            return RedirectToAction("Index", "Menu", new { restaurantId = foodItem.RestaurantId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveItem(int foodItemId)
        {
            if (foodItemId == 0) return BadRequest();
            var cart = GetCartItemsFromSession();
            var itemToRemove = cart.FirstOrDefault(item => item.FoodItemId == foodItemId);

            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCartItemsToSession(cart);
                TempData["SuccessMessage"] = "Item removed from cart.";
            }
            return RedirectToAction("Index"); // Redirects to the cart page (Cart/Index)
        }

        // Action to Increment Item Quantity
        [HttpGet] // GET istek
        public IActionResult IncrementItem(int foodItemId)
        {
            Debug.WriteLine($"IncrementItem çağrıldı: FoodItemId={foodItemId}");

            if (foodItemId == 0) return BadRequest();

            var cart = GetCartItemsFromSession();
            var cartItem = cart.FirstOrDefault(item => item.FoodItemId == foodItemId);

            if (cartItem != null)
            {
                cartItem.Quantity++; // Increment quantity
                SaveCartItemsToSession(cart);
                Debug.WriteLine($"Ürün miktarı artırıldı: FoodItemId={foodItemId}, Yeni miktar={cartItem.Quantity}");
            }
            else
            {
                TempData["ErrorMessage"] = "Ürün sepette bulunamadı.";
                Debug.WriteLine($"Sepette bulunamadı: FoodItemId={foodItemId}");
            }
            return RedirectToAction("Index");
        }

        // Action to Decrement Item Quantity (and remove if zero)
        [HttpGet] // GET istek
        public IActionResult DecrementItem(int foodItemId)
        {
            Debug.WriteLine($"DecrementItem çağrıldı: FoodItemId={foodItemId}");

            if (foodItemId == 0) return BadRequest();

            var cart = GetCartItemsFromSession();
            var cartItem = cart.FirstOrDefault(item => item.FoodItemId == foodItemId);

            if (cartItem != null)
            {
                cartItem.Quantity--; // Decrement quantity

                if (cartItem.Quantity <= 0)
                {
                    // Remove item if quantity is zero or less
                    cart.Remove(cartItem);
                    TempData["SuccessMessage"] = "Ürün sepetten kaldırıldı.";
                }

                SaveCartItemsToSession(cart);
                Debug.WriteLine($"Ürün miktarı azaltıldı: FoodItemId={foodItemId}, Yeni miktar={(cartItem.Quantity < 0 ? 0 : cartItem.Quantity)}");
            }
            else
            {
                TempData["ErrorMessage"] = "Ürün sepette bulunamadı.";
                Debug.WriteLine($"Sepette bulunamadı: FoodItemId={foodItemId}");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CartPageViewModel model)
        {
            try
            {
                Debug.WriteLine("Checkout işlemi başladı");
                var cartItems = GetCartItemsFromSession();

                // Sepet boş mu kontrolü
                if (!cartItems.Any())
                {
                    TempData["ErrorMessage"] = "Sepetiniz boş.";
                    return RedirectToAction("Index");
                }

                // Test amaçlı olarak Order veritabanı tablosunun yapısını incele
                Debug.WriteLine("Orders tablosu yapısını inceliyorum...");
                try
                {
                    var existingOrders = await _db.Orders.Take(1).ToListAsync();
                    if (existingOrders.Any())
                    {
                        Debug.WriteLine($"Mevcut sipariş örneği Id tipi: {existingOrders.First().Id.GetType().FullName}");
                    }
                    else
                    {
                        Debug.WriteLine("Orders tablosunda kayıt yok, tip bilgisi alınamadı.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Orders tablosu incelenirken hata: {ex.Message}");
                }

                // Basit bir sipariş oluştur
                var order = new Order
                {
                    OrderDate = DateTime.UtcNow,
                    OrderStatus = "Pending",
                    TotalAmount = cartItems.Sum(item => item.TotalPrice),
                    UserId = User.Identity?.Name ?? "Guest",
                    RestaurantId = cartItems.First().RestaurantId // RestaurantId'yi sepetteki ilk ürünün restoran ID'sinden al
                };

                Debug.WriteLine($"OrderDate: {order.OrderDate}, TotalAmount: {order.TotalAmount}, UserId: {order.UserId}, RestaurantId: {order.RestaurantId}");

                // Siparişi kaydet - önce orders tablosuna ekle
                _db.Orders.Add(order);
                var orderSaveResult = await _db.SaveChangesAsync();

                Debug.WriteLine($"Sipariş kaydedildi. OrderId: {order.Id}, Etkilenen satır: {orderSaveResult}");

                int successItemCount = 0;
                int failedItemCount = 0;

                // Sipariş öğelerini ekle
                foreach (var item in cartItems)
                {
                    try
                    {
                        // Veritabanından ürün bilgilerini al
                        var foodItem = await _db.FoodItemTables.FirstOrDefaultAsync(f => f.FoodItemId == item.FoodItemId);

                        if (foodItem != null)
                        {
                            // OrderItem nesnesi oluştur
                            var orderItem = new OrderItem
                            {
                                FoodItemId = foodItem.FoodItemId,
                                ProductName = foodItem.Name,
                                Quantity = item.Quantity,
                                Price = foodItem.Price,
                                OrderId = order.Id
                            };

                            Debug.WriteLine($"Eklenen sipariş öğesi: " +
                                           $"FoodItemId: {orderItem.FoodItemId} ({orderItem.FoodItemId.GetType().Name}), " +
                                           $"ProductName: {orderItem.ProductName}, " +
                                           $"Quantity: {orderItem.Quantity} ({orderItem.Quantity.GetType().Name}), " +
                                           $"Price: {orderItem.Price} ({orderItem.Price.GetType().Name}), " +
                                           $"OrderId: {orderItem.OrderId} ({orderItem.OrderId.GetType().Name})");

                            _db.OrderItems.Add(orderItem);

                            // Her öğeyi ayrı ayrı kaydet
                            var itemSaveResult = await _db.SaveChangesAsync();

                            if (itemSaveResult > 0)
                            {
                                Debug.WriteLine($"OrderItem başarıyla kaydedildi. ID: {orderItem.Id}");
                                successItemCount++;
                            }
                            else
                            {
                                Debug.WriteLine("OrderItem kaydedilirken bir sorun oluştu. Etkilenen satır: 0");
                                failedItemCount++;
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"UYARI: FoodItemId {item.FoodItemId} veritabanında bulunamadı!");
                            failedItemCount++;
                        }
                    }
                    catch (Exception itemEx)
                    {
                        Debug.WriteLine($"Sipariş öğesi eklenirken hata: {itemEx.Message}");
                        if (itemEx.InnerException != null)
                        {
                            Debug.WriteLine($"İç hata: {itemEx.InnerException.Message}");
                        }
                        failedItemCount++;
                        // Öğe eklenemedi, ancak diğer öğeleri eklemeye devam et
                        continue;
                    }
                }

                Debug.WriteLine($"Sipariş işlemi tamamlandı. Başarılı öğeler: {successItemCount}, Başarısız öğeler: {failedItemCount}");

                // Sepeti temizle
                SaveCartItemsToSession(new List<CartItemViewModel>());

                if (successItemCount > 0)
                {
                    TempData["SuccessMessage"] = "Siparişiniz başarıyla oluşturuldu!";
                    // Home/Index yerine OrderDetails sayfasına yönlendir
                    return RedirectToAction("OrderDetails", new { orderId = order.Id });
                }
                else
                {
                    TempData["WarningMessage"] = "Sipariş oluşturuldu ancak bazı ürünler eklenemedi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                // Hata bilgilerini yazdır
                Debug.WriteLine($"HATA: {ex.Message}");
                Debug.WriteLine($"HATA STACK: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"INNER HATA: {ex.InnerException.Message}");
                }

                // Kullanıcıya bilgi ver
                TempData["ErrorMessage"] = $"Sipariş oluşturulurken hata: {ex.Message}";

                return RedirectToAction("Index");
            }
        }

        // Veritabanı bağlantısını test etmek için yeni bir metot
        public IActionResult TestDatabase()
        {
            try
            {
                Debug.WriteLine("Veritabanı test işlemi başladı");

                // Veritabanı bağlantısını kontrol et
                bool canConnect = _db.Database.CanConnect();
                Debug.WriteLine($"Veritabanına bağlanılabilir mi: {canConnect}");

                // Migration durumunu kontrol et
                var migrations = _db.Database.GetAppliedMigrations().ToList();
                Debug.WriteLine($"Uygulanan migration sayısı: {migrations.Count}");

                foreach (var migration in migrations)
                {
                    Debug.WriteLine($"Uygulanan migration: {migration}");
                }

                // Tabloları kontrol et
                var orders = _db.Orders.ToList();
                Debug.WriteLine($"Mevcut sipariş sayısı: {orders.Count}");

                var items = _db.OrderItems.ToList();
                Debug.WriteLine($"Mevcut sipariş öğesi sayısı: {items.Count}");

                var foodItems = _db.FoodItemTables.Take(5).ToList();
                Debug.WriteLine($"İncelenen yemek sayısı: {foodItems.Count}");

                foreach (var item in foodItems)
                {
                    Debug.WriteLine($"FoodItem: Id={item.FoodItemId}, Name={item.Name}, Price={item.Price}");
                }

                // Manuel bir test siparişi oluştur
                var testOrder = new Order
                {
                    OrderDate = DateTime.UtcNow,
                    OrderStatus = "Test",
                    TotalAmount = 100,
                    UserId = "TestUser",
                    RestaurantId = 1 // Test için varsayılan RestaurantId
                };

                _db.Orders.Add(testOrder);
                int result = _db.SaveChanges();

                Debug.WriteLine($"Test siparişi kaydedildi. Etkilenen satır: {result}, Yeni OrderId: {testOrder.Id}");

                // Test siparişi için bir sipariş öğesi ekle
                if (testOrder.Id != Guid.Empty && foodItems.Any())
                {
                    var testOrderItem = new OrderItem
                    {
                        FoodItemId = foodItems.First().FoodItemId,
                        ProductName = "Test Product",
                        Quantity = 1,
                        Price = 100,
                        OrderId = testOrder.Id
                    };

                    _db.OrderItems.Add(testOrderItem);
                    int itemResult = _db.SaveChanges();

                    Debug.WriteLine($"Test sipariş öğesi kaydedildi. Etkilenen satır: {itemResult}, Yeni Id: {testOrderItem.Id}");
                }

                return Content("Veritabanı test edildi. Debug loglarını kontrol edin.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"TEST HATASI: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"İÇ HATA: {ex.InnerException.Message}");
                }

                return Content($"Veritabanı test hatası: {ex.Message}");
            }
        }

        // OrderDetails - Sipariş detaylarını gösterme
        public async Task<IActionResult> OrderDetails(Guid orderId)
        {
            try
            {
                // Siparişi veritabanından getir (restoran bilgisi dahil)
                var order = await _db.Orders
                    .Include(o => o.Restaurant)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Sipariş bulunamadı.";
                    return RedirectToAction("Index", "Home");
                }

                // Sipariş öğelerini getir
                var orderItems = await _db.OrderItems
                    .Where(oi => oi.OrderId == orderId)
                    .ToListAsync();

                // Sipariş model görünümü oluştur
                var model = new OrderDetailsViewModel
                {
                    Order = order,
                    OrderItems = orderItems
                };

                return View(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OrderDetails hatası: {ex.Message}");
                TempData["ErrorMessage"] = "Sipariş detayları görüntülenirken bir hata oluştu.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}