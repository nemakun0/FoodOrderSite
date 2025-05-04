using System;
using System.Collections.Generic;

namespace FoodOrderSite.Models.ViewModels
{
    public class RestaurantSettingsViewModel
    {
        public int UserId { get; set; }              // Controller kullanıyorsa bu gerekli
        public int RestaurantId { get; set; }        // Controller kullanıyorsa bu da gerekli

        public string RestaurantName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }

        public List<ScheduleDto> Schedules { get; set; }
    }

    public class ScheduleDto
    {
        public string DayOfWeek { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
    }
}
