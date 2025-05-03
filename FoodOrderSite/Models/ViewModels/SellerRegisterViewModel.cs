using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models.ViewModels
{
    public class SellerRegisterViewModel
    {
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
        public string RestaurantName { get; set; }

        [StringLength(50)]
        public string? RestaurantType { get; set; } // Örn: Kebapçı, Tatlıcı

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Required]
        [StringLength(50)]
        public string District { get; set; }
    }
}
