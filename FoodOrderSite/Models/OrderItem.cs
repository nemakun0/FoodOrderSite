using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        // İlişkili siparişin ID'si
        [Required]
        public int OrderId { get; set; }

        // Sipariş edilen ürünün (FoodItem) ID'si
        [Required]
        public int FoodItemId { get; set; }

        // Seçilen adet
        [Required]
        public int Quantity { get; set; }

        // Sipariş anındaki fiyat (FoodItem.Price kopyası)
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PriceAtOrderTime { get; set; }

        // Navigation Property - Order ilişkisi
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        // Navigation Property - FoodItem ilişkisi
        [ForeignKey("FoodItemId")]
        public virtual FoodItemTable FoodItem { get; set; }
    }
}
