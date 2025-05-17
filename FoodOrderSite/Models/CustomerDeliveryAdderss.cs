using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderSite.Models
{
    public class CustomerDeliveryAdderss
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        [ForeignKey("UserTable")]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(250)]
        public string AddressLine { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string District { get; set; }

        // Navigation property (optional, if you have a User model)
        public virtual UserTable User { get; set; }
    }
}
