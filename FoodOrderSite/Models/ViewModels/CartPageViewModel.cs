using System.Collections.Generic;
using System.Linq;

namespace FoodOrderSite.Models.ViewModels
{
    public class RestaurantCartGroupViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal RestaurantSubtotal => Items.Sum(item => item.TotalPrice);
    }

    public class CartPageViewModel
    {
        public List<RestaurantCartGroupViewModel> RestaurantGroups { get; set; } = new List<RestaurantCartGroupViewModel>();
        public decimal GrandTotal => RestaurantGroups.Sum(group => group.RestaurantSubtotal);
        public bool IsEmpty => !RestaurantGroups.Any() || RestaurantGroups.All(g => !g.Items.Any());

        // For payment section
        public string PaymentMethod { get; set; } // "Cash" or "CreditCard"
        public CreditCardViewModel CreditCard { get; set; } = new CreditCardViewModel();
        
        // Teslimat adresi bilgileri
        public int? DeliveryAddressId { get; set; }
        public string AddressTitle { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryDistrict { get; set; }
        
        // Kayıtlı adres listesi (controller tarafından doldurulacak)
        public List<CustomerDeliveryAddressViewModel> SavedAddresses { get; set; } = new List<CustomerDeliveryAddressViewModel>();
    }

    public class CreditCardViewModel
    {
        // Basic credit card fields - add DataAnnotations for validation later
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Cvv { get; set; }
        public string CardHolderName { get; set; }
    }
}