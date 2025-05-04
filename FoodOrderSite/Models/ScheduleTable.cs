using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSite.Models
{
    public class ScheduleTable
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        [ForeignKey("RestaurantTable")]
        public int RestaurantId { get; set; }

        [Required]
        public string DayOfWeek { get; set; } // Pazartesi, Salı, vs.

        [Required]
        public TimeSpan OpeningTime { get; set; }

        [Required]
        public TimeSpan ClosingTime { get; set; }

        public RestaurantTable? Restaurant { get; set; }
    }
}
