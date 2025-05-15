using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // Kullanıcıdan gelen ID (foreign key gibi kullanılabilir)
        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Adres ID değil doğrudan adresin kendisi alınabilir (CustomerDeliveryAddress tablosuyla ilişkili olabilir)
        [Required]
        public string DeliveryAddress { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        public string PaymentStatus { get; set; } = "unpaid"; // paid / unpaid

        [Required]
        public string OrderStatus { get; set; } = "preparing"; // preparing / shipped / delivered / cancelled

        // İlişki: Eğer User modeli varsa (Navigation Property)
        [ForeignKey("UserId")]
        public virtual UserTable User { get; set; }
    }
}
