using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models
{
    public class FoodItemTable
    {
        [Key]
        public int FoodItemId { get; set; }

        [ForeignKey("RestaurantTable")]
        public int RestaurantId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        [StringLength(255)]
        public string? Image { get; set; } // Dosya yolu ya da URL tutulabilir

        public bool IsAvailable { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        // Navigation property (opsiyonel)
        public virtual RestaurantTable? Restaurant { get; set; }
    }
}
