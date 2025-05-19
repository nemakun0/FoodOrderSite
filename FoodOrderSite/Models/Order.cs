using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSite.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public int UserId { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public int RestaurantId { get; set; }

        [ForeignKey("RestaurantId")]
        public RestaurantTable? Restaurant { get; set; }

        [StringLength(50)]
        public string OrderStatus { get; set; } = "Pending";

        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Cash";
        
        // Adres bilgileri
        public int? DeliveryAddressId { get; set; }
        
        [StringLength(100)]
        public string AddressTitle { get; set; }
        
        [StringLength(250)]
        public string DeliveryAddress { get; set; }
        
        [StringLength(100)]
        public string DeliveryCity { get; set; }
        
        [StringLength(100)]
        public string DeliveryDistrict { get; set; }
    }
}
