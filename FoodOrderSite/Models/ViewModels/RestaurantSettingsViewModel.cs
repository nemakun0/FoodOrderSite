using System.ComponentModel.DataAnnotations;
using FoodOrderSite.Models;

namespace FoodOrderSite.Models.ViewModels
{
    public class RestaurantSettingsViewModel
    {
        public int RestaurantId { get; set; }
        
        [Required(ErrorMessage = "Restoran adı gereklidir")]
        [StringLength(100)]
        public string RestaurantName { get; set; }
        
        [StringLength(50)]
        public string? RestaurantType { get; set; }
        
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
        
        [Required(ErrorMessage = "Telefon numarası gerekli")]
        [Phone(ErrorMessage = "Geçersiz telefon numarası")]
        public string Phone { get; set; }
        
        [Required(ErrorMessage = "Email boş olamaz")]
        [EmailAddress(ErrorMessage = "Geçersiz email adresi")]
        public string Email { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? Image { get; set; }

        public List<ScheduleViewModel> ScheduleItems { get; set; } = new List<ScheduleViewModel>();
    }
    
    public class ScheduleViewModel
    {
        public int? ScheduleId { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
    }
}
