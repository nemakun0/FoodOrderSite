using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        // **Yeni eklenenler**
        public bool IsBestSeller { get; set; } = false;
        public bool IsNew { get; set; } = false;
    }
}
