using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models
{
    public class RestaurantType
    {
        [Key]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "Store type name is required.")]
        [StringLength(50, ErrorMessage = "Store type name cannot exceed 50 characters.")]
        [Display(Name = "Store Type Name")]
        public string Name { get; set; }
    }
}
