using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models.ViewModels
{
    public class CustomerRegisterViewModel
    {
        public class RegisterViewModel
        {
            // UserTable fields
            [Required]
            public string Name { get; set; }

            [Required]
            public string Surname { get; set; }

            [Required]
            public DateTime BirthDate { get; set; }

            [Required]
            [EmailAddress]
            [Remote(action: "VerifyEmail", controller: "CustomerSignUp", ErrorMessage = "This email address is already registered.")]
            public string Email { get; set; }

            [Required]
            [Phone]
            [Remote(action: "VerifyPhone", controller: "CustomerSignUp", ErrorMessage = "This phone number is already registered.")]
            public string Phone { get; set; }

            [Required]
            [MinLength(6)]
            public string Password { get; set; }

            // Address fields
            [Required]
            [Display(Name = "Address Title")]
            [StringLength(100)]
            public string Title { get; set; }

            [Required]
            [Display(Name = "Address")]
            public string AddressLine { get; set; }

            [Required]
            [Display(Name = "City")]
            public string City { get; set; }

            [Required]
            [Display(Name = "District")]
            public string District { get; set; }

        }

    }
}
