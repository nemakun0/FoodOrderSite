using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models.ViewModels
{
    public class ProductAndMenuViewModel
    {
        public int? FoodItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int SelectedCategoryId { get; set; } // dropdown için seçili kategori

        public List<CategoryViewModel> AllCategories { get; set; } = new List<CategoryViewModel>();
        public List<FoodItemViewModel> ExistingProducts { get; set; } = new();
    }

    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }
    public class FoodItemViewModel
    {
        public int FoodItemId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
