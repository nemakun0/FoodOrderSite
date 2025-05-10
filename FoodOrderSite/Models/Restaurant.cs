// Models/Restaurant.cs
namespace FoodOrderSite.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RestaurantType { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string ShortDescription { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal MinOrderAmount { get; set; }
        public double Rating { get; set; }
        public string ImageUrl { get; set; }
        public int DeliveryTime { get; set; }
        public bool IsPromoted { get; set; }
    }
}