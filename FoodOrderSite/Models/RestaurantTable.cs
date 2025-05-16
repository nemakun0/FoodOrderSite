using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FoodOrderSite.Models
{
    public class RestaurantTable
    {
        [Key]
        public int RestaurantId { get; set; }
        [Required]
        [ForeignKey("UserTable")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Restoran adı gereklidir")]
        [StringLength(100)]
        public string RestaurantName { get; set; }
        public string? Image { get; set; }

        [StringLength(50)]
        public string? RestaurantType { get; set; } // Örn: Kebapçı, Tatlıcı

        [Required(ErrorMessage = "Adres bilgisi gereklidir")]
        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Şehir bilgisi gereklidir")]
        [StringLength(50)]
        public string City { get; set; }

        [Required(ErrorMessage = "İlçe bilgisi gereklidir")]
        [StringLength(50)]
        public string District { get; set; }

        public bool IsActive { get; set; } = true;
        

        // Navigation property
        public UserTable? User { get; set; }
    }
}
