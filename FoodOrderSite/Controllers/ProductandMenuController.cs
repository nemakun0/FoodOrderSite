using Microsoft.AspNetCore.Mvc;
using FoodOrderSite.Models;
using Microsoft.EntityFrameworkCore;
using FoodOrderSite.Models.ViewModels;
using System.Security.Claims;
using System.Linq;
using static FoodOrderSite.Models.ViewModels.ProductAndMenuViewModel;
using Newtonsoft.Json;

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

                // Kategori ilişkilendirmesi (FoodItemCategoriesTable'a kayıt)
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

            // Kategori seçimi kontrolü
            if (model.SelectedCategoryId <= 0) // Assuming 0 or less indicates no category is selected
            {
                // Hata mesajını TempData'ya kaydet
                TempData["CategorySelectionError"] = "Lütfen bir kategori seçin.";
                
                // Kullanıcının girdiği verileri TempData'ya kaydet
                var sanitizedName = model.Name?.Replace("\"", "\\\"").Replace("'", "\\'");
                var sanitizedDescription = model.Description?.Replace("\"", "\\\"").Replace("'", "\\'");
                
                TempData["EditProductData"] = JsonConvert.SerializeObject(new
                {
                    model.FoodItemId,
                    Name = sanitizedName,
                    Description = sanitizedDescription,
                    model.Price,
                    model.IsAvailable
                });
                
                // Index sayfasına yönlendir
                return RedirectToAction("Index");
            }

            _context.FoodItemTables.Update(existingProduct);
            await _context.SaveChangesAsync();

            // FoodItemCategoriesTable'da mevcut kategoriyi güncelle
            var existingCategoryRelation = await _context.FoodItemCategoriesTables
                .FirstOrDefaultAsync(rel => rel.FoodItemId == model.FoodItemId);

            if (existingCategoryRelation != null)
            {
                // Eğer kategori değişmişse, eski ilişkiyi sil ve yenisini ekle
                if (existingCategoryRelation.CategoryId != model.SelectedCategoryId)
                {
                    _context.FoodItemCategoriesTables.Remove(existingCategoryRelation);
                    await _context.SaveChangesAsync(); // Değişiklikleri kaydet

                    var newCategoryRelation = new FoodItemCategoriesTable
                    {
                        FoodItemId = existingProduct.FoodItemId,
                        CategoryId = model.SelectedCategoryId
                    };
                    _context.FoodItemCategoriesTables.Add(newCategoryRelation);
                }
                // Kategori değişmemişse bir şey yapmaya gerek yok.
            }
            else
            {
                // Eğer daha önce bir kategori ilişkisi yoksa, yenisini oluştur
                var newCategoryRelation = new FoodItemCategoriesTable
                {
                    FoodItemId = existingProduct.FoodItemId,
                    CategoryId = model.SelectedCategoryId
                };
                _context.FoodItemCategoriesTables.Add(newCategoryRelation);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Veya istediğiniz sayfaya yönlendirebilirsiniz
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFoodItem(int foodItemId)
        {
            var product = await _context.FoodItemTables
                .FirstOrDefaultAsync(p => p.FoodItemId == foodItemId);

            if (product == null)
            {
                return NotFound("Ürün bulunamadı.");
            }

            // Soft delete
            product.IsDeleted = true;
            _context.FoodItemTables.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        //public IActionResult Index()
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<FoodItemViewModel> productViewModels = new List<FoodItemViewModel>();
            RestaurantTable userRestaurant = null;

            // Check if there's edit data with validation errors in TempData
            if (TempData["EditProductData"] != null && TempData["CategorySelectionError"] != null)
            {
                var editData = JsonConvert.DeserializeObject<dynamic>(TempData["EditProductData"].ToString());
                var errorMessage = TempData["CategorySelectionError"].ToString();
                
                // Pass the validation error to the view
                ModelState.AddModelError("SelectedCategoryId", errorMessage);
                
                // Signal to the view that it should open the edit modal with the product ID
                ViewData["OpenEditModal"] = true;
                ViewData["EditProductId"] = (int)editData.FoodItemId;
                ViewData["EditProductName"] = (string)editData.Name;
                ViewData["EditProductDescription"] = (string)editData.Description;
                ViewData["EditProductPrice"] = (decimal)editData.Price;
                ViewData["EditProductIsAvailable"] = (bool)editData.IsAvailable;
            }

            if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
            {
                // Find the restaurant associated with the current user
                userRestaurant = await _context.RestaurantTables
                                             .FirstOrDefaultAsync(r => r.UserId == userId && r.IsActive);

                if (userRestaurant != null)
                {
                    // Fetch products only for the user's restaurant
                    var products = await _context.FoodItemTables
                        .Where(p => p.RestaurantId == userRestaurant.RestaurantId)
                        .OrderBy(p => p.Name)
                        .ToListAsync();

                    productViewModels = products
                        .Select(p => new FoodItemViewModel
                        {
                            FoodItemId = p.FoodItemId,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.Price,
                            Image = p.Image,
                            IsAvailable = p.IsAvailable,
                            IsDeleted = p.IsDeleted
                        }).ToList();
                }
            }

            // Get categories for the view
            var categories = await _context.CategoriesTables.ToListAsync();
            var categoryViewModels = categories
                .Select(c => new CategoryViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
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