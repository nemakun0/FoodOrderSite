using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodOrderSite.Models;

namespace FoodOrderSite.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.CategoriesTables.ToList();
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(string name, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return Json(new { success = false, message = "Kategori adı zorunludur." });
                }

                var category = new CategoriesTable
                {
                    Name = name,
                    Description = description ?? ""
                };

                _context.CategoriesTables.Add(category);
                _context.SaveChanges();

                return Json(new { success = true, message = "Kategori başarıyla eklendi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int categoryId)
        {
            try
            {
                var category = _context.CategoriesTables.Find(categoryId);
                if (category == null)
                {
                    return Json(new { success = false, message = "Kategori bulunamadı" });
                }

                _context.CategoriesTables.Remove(category);
                _context.SaveChanges();

                return Json(new { success = true, message = "Kategori başarıyla silindi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetCategory(int categoryId)
        {
            var category = _context.CategoriesTables.Find(categoryId);
            if (category == null)
            {
                return Json(new { success = false, message = "Kategori bulunamadı" });
            }

            return Json(new { success = true, category = category });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCategory(int categoryId, string name, string description)
        {
            try
            {
                var category = _context.CategoriesTables.Find(categoryId);
                if (category == null)
                {
                    return Json(new { success = false, message = "Kategori bulunamadı" });
                }

                category.Name = name;
                category.Description = description ?? "";

                _context.Entry(category).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new { success = true, message = "Kategori başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }
    }
} 