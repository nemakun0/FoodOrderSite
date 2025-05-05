using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSite.Models
{
    public class FoodItemCategoriesTable
    {
        // [Key] attribute'lerini KALDIRIN
        public int FoodItemId { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public virtual FoodItemTable? FoodItem { get; set; }
        public virtual CategoriesTable? Category { get; set; }
    }
}