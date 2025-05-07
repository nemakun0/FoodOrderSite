namespace FoodOrderSite.ViewModels
{
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
    }
}