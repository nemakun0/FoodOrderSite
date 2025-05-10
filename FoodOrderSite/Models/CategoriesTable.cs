using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSite.Models
{
    [Table("CategoriesTables")]
    public class CategoriesTable
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // İlişkili FoodItemCategories varsa aşağıdaki navigation property eklenebilir
        // public virtual ICollection<FoodItemCategory> FoodItemCategories { get; set; }
    }
}
