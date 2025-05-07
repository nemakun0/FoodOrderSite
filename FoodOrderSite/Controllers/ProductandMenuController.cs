using Microsoft.AspNetCore.Mvc;
using FoodOrderSite.Models;
using Microsoft.EntityFrameworkCore;
using FoodOrderSite.Models.ViewModels;
using System.Security.Claims;
using static FoodOrderSite.Models.ViewModels.ProductAndMenuViewModel;

namespace FoodOrderSite.Controllers
{
    public class ProductandMenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductandMenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.CategoriesTables
                .Select(c => new CategoryViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                }).ToList();

            var viewModel = new ProductAndMenuViewModel
            {
                AllCategories = categories
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductAndMenuViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Sayfa tekrar yüklendiğinde kategoriler yeniden yüklenmeli
                model.AllCategories = _context.CategoriesTables
                    .Select(c => new CategoryViewModel
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name
                    }).ToList();

                return View(model);
            }
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ImageFile.CopyTo(fileStream);
                    }
                }

                // Kullanıcı ID'sini claim'den al
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var restaurant = await _context.RestaurantTables
                    .FirstOrDefaultAsync(r => r.UserId == int.Parse(userId));

                if (restaurant == null)
                    return NotFound("Restoran bulunamadı.");

                var newItem = new FoodItemTable
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Image = uniqueFileName != null ? "/uploads/" + uniqueFileName : null,
                    IsAvailable = model.IsAvailable,
                    RestaurantId = restaurant.RestaurantId
                };


                _context.FoodItemTables.Add(newItem);
                await _context.SaveChangesAsync();

                // Kategori ilişkilendirmesi (FoodItemCategoriesTable’a kayıt)
                var categoryRelation = new FoodItemCategoriesTable
                {
                    FoodItemId = newItem.FoodItemId,
                    CategoryId = model.SelectedCategoryId
                };

                _context.FoodItemCategoriesTables.Add(categoryRelation);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            // Bu satır if'lerin dışında kalan durumları kapsar
            model.AllCategories = _context.CategoriesTables
                .Select(c => new CategoryViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductAndMenuViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Kategoriler yeniden yüklenmeli
                model.AllCategories = _context.CategoriesTables
                    .Select(c => new CategoryViewModel
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name
                    }).ToList();
                return View(model);
            }


            if (model.FoodItemId == 0) // Geçerli bir FoodItemId bekleniyor
            {
                Console.WriteLine("Gelen FoodItemId: " + model.FoodItemId);
                return BadRequest("Geçersiz ürün ID'si gönderildi.");
            }

            // Ürünü veritabanında bul
            var existingProduct = await _context.FoodItemTables
                .FirstOrDefaultAsync(p => p.FoodItemId == model.FoodItemId);

            if (existingProduct == null)
            {
                return NotFound("Ürün bulunamadı.");
            }

            // Veritabanı ürününü güncelle
            existingProduct.Name = model.Name;
            existingProduct.Description = model.Description;
            existingProduct.Price = model.Price;
            existingProduct.IsAvailable = model.IsAvailable;

            if (model.ImageFile != null)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);
                }

                existingProduct.Image = "/uploads/" + uniqueFileName;
            }

            _context.FoodItemTables.Update(existingProduct);
            await _context.SaveChangesAsync();

            // FoodItemCategoriesTable'da mevcut kategoriyi güncelle
            var existingCategoryRelation = await _context.FoodItemCategoriesTables
                .FirstOrDefaultAsync(rel => rel.FoodItemId == model.FoodItemId);

            if (existingCategoryRelation != null)
            {
                // Kategoriyi güncelle
                existingCategoryRelation.CategoryId = model.SelectedCategoryId;
                _context.FoodItemCategoriesTables.Update(existingCategoryRelation);
            }
            else
            {
                // Eğer ilişki yoksa hata döndür
                return BadRequest("Bu ürün için bir kategori ilişkisi bulunmamaktadır.");
            }

            //_context.FoodItemTables.Update(existingProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Veya istediğiniz sayfaya yönlendirebilirsiniz
        }
        public IActionResult Index()
        {
            var categories = _context.CategoriesTables.ToList();

            var categoryViewModels = categories
                .Select(c => new CategoryViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                }).ToList();
            // Ürünleri çek
            var products = _context.FoodItemTables.ToList();

            var productViewModels = products
                .Select(p => new FoodItemViewModel
                {
                    FoodItemId = p.FoodItemId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Image = p.Image,
                    IsAvailable = p.IsAvailable
                }).ToList();

            var viewModel = new ProductAndMenuViewModel
            {
                AllCategories = categoryViewModels,
                ExistingProducts = productViewModels
            };

            return View(viewModel);
        }
    }
}