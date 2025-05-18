using System;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models.ViewModels
{
    public class CreateReviewViewModel
    {
        public Guid OrderId { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public DateTime OrderDate { get; set; }
        
        [Range(1, 5, ErrorMessage = "Lezzet puanı 1-5 arasında olmalıdır.")]
        [Required(ErrorMessage = "Lezzet puanı gereklidir.")]
        public int TasteRating { get; set; }
        
        [Range(1, 5, ErrorMessage = "Servis puanı 1-5 arasında olmalıdır.")]
        [Required(ErrorMessage = "Servis puanı gereklidir.")]
        public int ServiceRating { get; set; }
        
        [Range(1, 5, ErrorMessage = "Teslimat puanı 1-5 arasında olmalıdır.")]
        [Required(ErrorMessage = "Teslimat puanı gereklidir.")]
        public int DeliveryRating { get; set; }
        
        [Range(1, 5, ErrorMessage = "Genel değerlendirme puanı 1-5 arasında olmalıdır.")]
        [Required(ErrorMessage = "Genel değerlendirme puanı gereklidir.")]
        public int OverallRating { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Yorum 1000 karakterden uzun olamaz.")]
        public string Comment { get; set; }
        
        // Navigation or retrieval properties
        public bool HasReview { get; set; }
    }
} 