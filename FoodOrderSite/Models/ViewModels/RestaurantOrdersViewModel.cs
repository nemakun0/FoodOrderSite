using System;
using System.Collections.Generic;
using FoodOrderSite.Models;
using System.Linq;

namespace FoodOrderSite.Models.ViewModels
{
    public class RestaurantOrdersViewModel
    {
        public List<OrderWithDetails> Orders { get; set; } = new List<OrderWithDetails>();
        public string RestaurantName { get; set; }
        public int RestaurantId { get; set; }
        public string LastUpdatedOrderId { get; set; }
    }

    public class OrderWithDetails
    {
        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public UserTable Customer { get; set; }
        
        // Helper properties for easy access in view
        public string OrderId => Order.Id.ToString();
        public string OrderShortId => Order.Id.ToString().Substring(0, 8).ToUpper();
        public DateTime OrderDate => Order.OrderDate;
        public string FormattedOrderDate => Order.OrderDate.ToString("yyyy-MM-dd");
        public string FormattedOrderTime => Order.OrderDate.ToString("HH:mm");
        public string OrderStatus => Order.OrderStatus;
        public decimal TotalAmount => Order.TotalAmount;
        public string CustomerName => $"{Customer?.Name} {Customer?.Surname}";
        public string CustomerPhone => Customer?.Phone;
        public string CustomerEmail => Customer?.Email;
        
        // Adres bilgileri için yardımcı özellikler
        public string PaymentMethod => Order.PaymentMethod;
        public int? DeliveryAddressId => Order.DeliveryAddressId;
        public string AddressTitle => Order.AddressTitle;
        public string DeliveryAddress => Order.DeliveryAddress;
        public string DeliveryCity => Order.DeliveryCity;
        public string DeliveryDistrict => Order.DeliveryDistrict;
        
        // Adres bilgisini birleşik olarak görüntüle
        public string FullAddress => $"{DeliveryAddress}, {DeliveryDistrict} / {DeliveryCity}";
        public bool HasAddressInfo => !string.IsNullOrEmpty(DeliveryAddress);
    }
} 