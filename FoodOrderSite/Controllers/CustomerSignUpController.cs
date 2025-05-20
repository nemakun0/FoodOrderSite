using Microsoft.AspNetCore.Mvc;
using FoodOrderSite.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using static FoodOrderSite.Models.ViewModels.CustomerRegisterViewModel;

namespace FoodOrderSite.Controllers
{
    public class CustomerSignUpController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerSignUpController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // ✅ Email check
                if (_context.UserTables.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(model);
                }

                // ✅ Phone check
                if (_context.UserTables.Any(u => u.Phone == model.Phone))
                {
                    ModelState.AddModelError("Phone", "This phone number is already registered.");
                    return View(model);
                }
                // 1. Create UserTable model
                var user = new UserTable
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    BirthDate = model.BirthDate,
                    Email = model.Email,
                    Phone = model.Phone,
                    Password = model.Password, // Note: Should be hashed
                    Role = "customer"          // Set role as "customer"
                };

                // Add user to database
                _context.UserTables.Add(user);
                await _context.SaveChangesAsync();

                // 2. Create CustomerDeliveryAddress model
                var address = new CustomerDeliveryAdderss
                {
                    UserId = user.UserId, // ID of the user we just created
                    Title = model.Title ?? "",
                    AddressLine = model.AddressLine,
                    City = model.City,
                    District = model.District
                };

                // Debugging: Log Title value
                System.Diagnostics.Debug.WriteLine($"Saved Address Title: '{model.Title}'");

                // Add address to database
                _context.CustomerDeliveryAdderss.Add(address);
                await _context.SaveChangesAsync();

                // Redirect to another page if registration is successful
                return RedirectToAction("Index", "SignIn"); // Redirect to success page if registration is successful
            }

            // If model is not valid, show the form again
            return View(model);
        }

        // Remote action for phone number validation
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyPhone(string phone)
        {
            if (_context.UserTables.Any(u => u.Phone == phone))
            {
                return Json($"This phone number is already registered: {phone}");
            }
            return Json(true);
        }

        // Remote action for email validation
        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyEmail(string email)
        {
            if (_context.UserTables.Any(u => u.Email == email))
            {
                return Json($"This email address is already registered: {email}");
            }
            return Json(true);
        }
    }
}
