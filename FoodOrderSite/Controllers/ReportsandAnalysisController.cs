using Microsoft.AspNetCore.Mvc;
using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace FoodOrderSite.Controllers
{
    public class ReportsandAnalysisController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReportsandAnalysisController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Oturum kontrolü
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "SignIn");
            }

            // Kullanıcı ID'sini al
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Kullanıcının restoran bilgilerini al
            var restaurant = await _db.RestaurantTables
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (restaurant == null)
            {
                // Restoran bulunamadıysa ana sayfaya yönlendir
                return RedirectToAction("Index", "Home");
            }

            // Restoran için değerlendirmeleri al
            var reviews = await _db.ReviewTable
                .Include(r => r.User)
                .Include(r => r.Order)
                .Where(r => r.RestaurantId == restaurant.RestaurantId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            // View Model oluştur
            var viewModel = new RestaurantReviewsViewModel
            {
                RestaurantId = restaurant.RestaurantId,
                RestaurantName = restaurant.RestaurantName,
                Reviews = new List<ReviewDetailViewModel>(),
                TotalReviews = reviews.Count
            };

            // Yoksa boş model döndür
            if (!reviews.Any())
            {
                return View(viewModel);
            }

            // Değerlendirmeleri model'e ekle
            foreach (var review in reviews)
            {
                viewModel.Reviews.Add(new ReviewDetailViewModel
                {
                    ReviewId = review.ReviewId,
                    CreatedAt = review.CreatedAt,
                    TasteRating = review.TasteRating,
                    ServiceRating = review.ServiceRating,
                    DeliveryRating = review.DeliveryRating,
                    OverallRating = review.OverallRating,
                    Comment = review.Comment ?? "",
                    UserId = review.UserId,
                    UserName = review.User?.Name + " " + review.User?.Surname,
                    OrderShortId = review.OrderId.ToString().Substring(0, 8).ToUpper()
                });
            }

            // Ortalama puanları hesapla
            viewModel.AverageTasteRating = reviews.Average(r => r.TasteRating);
            viewModel.AverageServiceRating = reviews.Average(r => r.ServiceRating);
            viewModel.AverageDeliveryRating = reviews.Average(r => r.DeliveryRating);
            viewModel.AverageOverallRating = reviews.Average(r => r.OverallRating);

            // Yıldız sayılarını hesapla
            viewModel.FiveStarCount = reviews.Count(r => r.OverallRating == 5);
            viewModel.FourStarCount = reviews.Count(r => r.OverallRating == 4);
            viewModel.ThreeStarCount = reviews.Count(r => r.OverallRating == 3);
            viewModel.TwoStarCount = reviews.Count(r => r.OverallRating == 2);
            viewModel.OneStarCount = reviews.Count(r => r.OverallRating == 1);

            // Satış ve ürün verilerini içeren model oluştur
            var salesViewModel = await GetSalesAndProductsData(restaurant.RestaurantId);

            // İki modeli birleştir
            ViewBag.SalesViewModel = salesViewModel;

            return View(viewModel);
        }

        private async Task<SalesAndProductsViewModel> GetSalesAndProductsData(int restaurantId)
        {
            var viewModel = new SalesAndProductsViewModel
            {
                RestaurantId = restaurantId,
                RestaurantName = (await _db.RestaurantTables.FindAsync(restaurantId))?.RestaurantName ?? ""
            };

            // Tarih aralıklarını belirle
            var today = DateTime.Now.Date;
            // Türkiye'de hafta Pazartesi başlar (DayOfWeek.Monday = 1)
            int dayOfWeek = (int)today.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7; // Pazar (0) -> 7 olarak ayarla
            var startOfWeek = today.AddDays(-(dayOfWeek - 1)); // Pazartesi'ye ayarla
            var endOfWeek = startOfWeek.AddDays(6); // Pazar gününe
            var startOfYear = new DateTime(today.Year, 1, 1);

            // Günlük satış verileri (saat dilimlerine göre)
            var timeSlots = new[] { "09:00", "12:00", "15:00", "18:00", "21:00" };
            
            var dailyOrders = await _db.Orders
                .Where(o => o.RestaurantId == restaurantId && o.OrderDate.Date == today)
                .ToListAsync();

            foreach (var slot in timeSlots)
            {
                var hour = int.Parse(slot.Split(':')[0]);
                decimal amount = 0;
                
                if (dailyOrders.Any())
                {
                    // Her saat dilimi için o saate kadar olan satışları topla
                    var slotStart = new DateTime(today.Year, today.Month, today.Day, hour, 0, 0);
                    var slotEnd = (hour < 21) ? new DateTime(today.Year, today.Month, today.Day, hour + 3, 0, 0) : today.AddDays(1);
                    
                    amount = dailyOrders
                        .Where(o => o.OrderDate >= slotStart && o.OrderDate < slotEnd)
                        .Sum(o => o.TotalAmount);
                }
                
                viewModel.DailySales.Add(new TimeSlotSale { TimeSlot = slot, Amount = amount });
            }

            // Haftalık satış verileri
            string[] dayNames = { "Pzt", "Sal", "Çar", "Per", "Cum", "Cmt", "Paz" };

            // Debug - Haftanın başlangıcı ve sonu
            System.Diagnostics.Debug.WriteLine($"Hafta başlangıcı: {startOfWeek.ToString("yyyy-MM-dd")}, Hafta sonu: {endOfWeek.ToString("yyyy-MM-dd")}");

            var weeklyOrders = await _db.Orders
                .Where(o => o.RestaurantId == restaurantId && o.OrderDate.Date >= startOfWeek.Date && o.OrderDate.Date <= endOfWeek.Date)
                .ToListAsync();

            // Her gün için değerleri ekle (veri olmasa bile)
            for (int i = 0; i < 7; i++)
            {
                var currentDay = startOfWeek.AddDays(i);
                decimal amount = 0;
                
                if (weeklyOrders.Any())
                {
                    // O güne ait siparişleri topla
                    amount = weeklyOrders
                        .Where(o => o.OrderDate.Date == currentDay.Date)
                        .Sum(o => o.TotalAmount);
                }
                
                // Debug - eklenen günlük veri
                System.Diagnostics.Debug.WriteLine($"Gün: {dayNames[i]}, Tarih: {currentDay.ToString("yyyy-MM-dd")}, Tutar: {amount}");
                
                viewModel.WeeklySales.Add(new DaySale { DayName = dayNames[i], Amount = amount });
            }

            // Aylık satış verileri
            string[] monthNames = CultureInfo.GetCultureInfo("tr-TR").DateTimeFormat.MonthNames.Take(12).ToArray();
            
            for (int i = 0; i < 12; i++)
            {
                var startOfMonth = new DateTime(today.Year, i + 1, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                
                var monthlyAmount = await _db.Orders
                    .Where(o => o.RestaurantId == restaurantId && o.OrderDate >= startOfMonth && o.OrderDate <= endOfMonth)
                    .SumAsync(o => o.TotalAmount);
                
                viewModel.MonthlySales.Add(new MonthSale { MonthName = monthNames[i], Amount = monthlyAmount });
            }

            // En çok satan ürünleri al
            var topProducts = await _db.OrderItems
                .Join(_db.Orders, item => item.OrderId, order => order.Id, (item, order) => new { Item = item, Order = order })
                .Where(x => x.Order.RestaurantId == restaurantId)
                .GroupBy(x => new { x.Item.FoodItemId, x.Item.ProductName })
                .Select(g => new
                {
                    FoodItemId = g.Key.FoodItemId,
                    ProductName = g.Key.ProductName,
                    SalesCount = g.Sum(x => x.Item.Quantity)
                })
                .OrderByDescending(x => x.SalesCount)
                .Take(2)
                .ToListAsync();

            foreach (var product in topProducts)
            {
                viewModel.TopProducts.Add(new TopProductViewModel
                {
                    FoodItemId = product.FoodItemId,
                    ProductName = product.ProductName,
                    SalesCount = product.SalesCount
                });
            }

            // En çok satan kategorileri al
            var topCategories = await _db.OrderItems
                .Join(_db.Orders, item => item.OrderId, order => order.Id, (item, order) => new { Item = item, Order = order })
                .Where(x => x.Order.RestaurantId == restaurantId)
                .Join(_db.FoodItemCategoriesTables, x => x.Item.FoodItemId, fic => fic.FoodItemId, (x, fic) => new { x.Item, x.Order, FoodItemCategory = fic })
                .Join(_db.CategoriesTables, x => x.FoodItemCategory.CategoryId, c => c.CategoryId, (x, category) => new { x.Item, x.Order, Category = category })
                .GroupBy(x => new { x.Category.CategoryId, x.Category.Name })
                .Select(g => new
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    SalesCount = g.Sum(x => x.Item.Quantity)
                })
                .OrderByDescending(x => x.SalesCount)
                .Take(2)
                .ToListAsync();

            foreach (var category in topCategories)
            {
                viewModel.TopCategories.Add(new TopCategoryViewModel
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    SalesCount = category.SalesCount
                });
            }

            // Toplam gelir
            viewModel.TotalIncome = await _db.Orders
                .Where(o => o.RestaurantId == restaurantId)
                .SumAsync(o => o.TotalAmount);

            // Satış verisi olup olmadığını kontrol et
            viewModel.HasAnySalesData = viewModel.TotalIncome > 0;

            // Kar marjını hesapla (örnek, gerçek iş mantığınıza göre ayarlayın)

            return viewModel;
        }
    }
}
