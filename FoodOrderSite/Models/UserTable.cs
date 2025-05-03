using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models
{
    public class UserTable
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Ad boş olamaz")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyad boş olamaz")]
        [StringLength(50)]
        public string Surname { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Email boş olamaz")]
        [EmailAddress(ErrorMessage = "Geçersiz email adresi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon numarası gerekli")]
        [Phone(ErrorMessage = "Geçersiz telefon numarası")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Şifre boş olamaz")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalı")]
        public string Password { get; set; }
        public string Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool Status { get; set; } = true; // true = aktif, false = pasif

        // İlişkiler (isteğe bağlı eklenebilir)
        // public ICollection<Restaurant> Restaurants { get; set; }
        // public ICollection<Cart> Carts { get; set; }

    }
}
