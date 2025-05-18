using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSite.Models
{
    [Table("Reviews")]
    public class ReviewTable
    {
        [Key]
        public int ReviewId { get; set; }

        public Guid OrderId { get; set; }   // Guid olduğu için burada da Guid olmalı
        public int UserId { get; set; }
        public int RestaurantId { get; set; }

        // Puanlamalar
        [Range(1, 5)]
        public int TasteRating { get; set; }

        [Range(1, 5)]
        public int ServiceRating { get; set; }

        [Range(1, 5)]
        public int DeliveryRating { get; set; }

        [Range(1, 5)]
        public int OverallRating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // === Navigation Properties ===

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [ForeignKey("RestaurantId")]
        public RestaurantTable? Restaurant { get; set; }

        [ForeignKey("UserId")]
        public UserTable? User { get; set; }
    }
}
