using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models
{
    public class CategoriesTable
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // İlişkili FoodItemCategories varsa aşağıdaki navigation property eklenebilir
        // public virtual ICollection<FoodItemCategory> FoodItemCategories { get; set; }
    }
}
