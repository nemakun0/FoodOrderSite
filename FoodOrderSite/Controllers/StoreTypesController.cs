using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodOrderSite.Models;
//using FoodOrderSite.Data; bu kod hata veriyor düzeltme

namespace FoodOrderSite.Controllers
{
    public class StoreTypesController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public StoreTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var types = await _context.RestaurantTypes.ToListAsync();
            return View(types);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] RestaurantType restaurantType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(restaurantType);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Mağaza türü başarıyla eklendi.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("TypeId,Name")] RestaurantType restaurantType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingType = await _context.RestaurantTypes.FindAsync(restaurantType.TypeId);
                    if (existingType == null)
                    {
                        return NotFound();
                    }

                    existingType.Name = restaurantType.Name;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Mağaza türü başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantTypeExists(restaurantType.TypeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var restaurantType = await _context.RestaurantTypes.FindAsync(id);
            if (restaurantType != null)
            {
                _context.RestaurantTypes.Remove(restaurantType);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Mağaza türü başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantTypeExists(int id)
        {
            return _context.RestaurantTypes.Any(e => e.TypeId == id);
        }
    }
} 