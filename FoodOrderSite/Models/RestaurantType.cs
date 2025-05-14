using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models
{
    public class RestaurantType
    {
        [Key]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "Mağaza türü adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Mağaza türü adı en fazla 50 karakter olabilir.")]
        [Display(Name = "Mağaza Türü Adı")]
        public string Name { get; set; }
    }
}
