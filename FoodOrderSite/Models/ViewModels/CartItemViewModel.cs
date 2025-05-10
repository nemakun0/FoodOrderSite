namespace FoodOrderSite.Models.ViewModels // Assuming this is the namespace from Menu/Index.cshtml
{
    public class CartItemViewModel
    {
        public int FoodItemId { get; set; }
        public string FoodItemName { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } 
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public string ImageUrl { get; set; } // Added for display in cart

        // Parameterless constructor for deserialization if needed
        public CartItemViewModel() {}

        // Constructor for easier creation
        public CartItemViewModel(FoodItemTable foodItem, RestaurantTable restaurant, int quantity = 1)
        {
            FoodItemId = foodItem.FoodItemId;
            FoodItemName = foodItem.Name;
            RestaurantId = restaurant.RestaurantId;
            RestaurantName = restaurant.RestaurantName;
            Quantity = quantity;
            UnitPrice = foodItem.Price;
            ImageUrl = foodItem.Image; // Assuming FoodItemTable has an Image property
        }
    }
} 