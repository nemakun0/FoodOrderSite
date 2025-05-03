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
            public string Email { get; set; }

            [Required]
            [Phone]
            public string Phone { get; set; }

            [Required]
            [MinLength(6)]
            public string Password { get; set; }

            // Address alanları
            [Required]
            public string AddressLine { get; set; }

            [Required]
            public string City { get; set; }

            [Required]
            public string District { get; set; }
        }

    }
}
