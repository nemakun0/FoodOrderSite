using Microsoft.AspNetCore.Mvc;
using FoodOrderSite.Models;
using FoodOrderSite.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderSite.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new UserManagementViewModel
            {
                Customers = _context.UserTables.Where(u => u.Role == "customer").ToList(),
                Sellers = _context.UserTables.Where(u => u.Role == "seller").ToList()
            };

            ViewBag.CustomerCount = viewModel.Customers.Count;
            ViewBag.SellerCount = viewModel.Sellers.Count;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ToggleStatus(int userId, string userType, bool status)
        {
            try
            {
                var user = _context.UserTables.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı" });
                }

                user.Status = status;
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new { success = true, message = "Durum başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }
    }
}