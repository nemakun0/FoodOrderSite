using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models.ViewModels
{
    public class CustomerRegisterViewModel
    {
        public class RegisterViewModel
        {
            // UserTable alanları
            [Required]
            public string Name { get; set; }

            [Required]
            public string Surname { get; set; }

            [Required]
            public DateTime BirthDate { get; set; }

            [Required]
            [EmailAddress]
            [Remote(action: "VerifyEmail", controller: "CustomerSignUp", ErrorMessage = "Bu email adresi zaten kayıtlı.")]
            public string Email { get; set; }

            [Required]
            [Phone]
            [Remote(action: "VerifyPhone", controller: "CustomerSignUp", ErrorMessage = "Bu telefon numarası zaten kayıtlı.")]
            public string Phone { get; set; }

            [Required]
            [MinLength(6)]
            public string Password { get; set; }

            // Address alanları
            [Required]
            [Display(Name = "Adres Başlığı")]
            public string AddressTitle { get; set; }

            [Required]
            [Display(Name = "Adres")]
            public string AddressLine { get; set; }

            [Required]
            [Display(Name = "Şehir")]
            public string City { get; set; }

            [Required]
            [Display(Name = "İlçe")]
            public string District { get; set; }
        }

    }
}
