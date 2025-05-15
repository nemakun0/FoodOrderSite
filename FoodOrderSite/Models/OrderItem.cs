using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSite.Models
{
    [Table("OrderItems")]
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int FoodItemId { get; set; }

        // Sipariþ edilen ürünün (FoodItem) ID'si
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        // Seçilen adet
        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        public Guid OrderId { get; set; }
    }
}
