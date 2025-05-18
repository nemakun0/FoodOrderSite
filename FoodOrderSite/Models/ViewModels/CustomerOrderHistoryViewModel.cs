using System;
using System.Collections.Generic;
using FoodOrderSite.Models;

namespace FoodOrderSite.Models.ViewModels
{
    public class CustomerOrderHistoryViewModel
    {
        public List<CustomerOrderViewModel> Orders { get; set; } = new List<CustomerOrderViewModel>();
        public string CurrentFilter { get; set; }
    }

    public class CustomerOrderViewModel
    {
        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public bool HasReview { get; set; }
        
        // Helper properties for easy access in view
        public string OrderId => Order.Id.ToString();
        public string OrderShortId => Order.Id.ToString().Substring(0, 8).ToUpper();
        public DateTime OrderDate => Order.OrderDate;
        public string FormattedOrderDate => Order.OrderDate.ToString("yyyy-MM-dd");
        public string FormattedOrderTime => Order.OrderDate.ToString("HH:mm");
        public string OrderStatus => Order.OrderStatus;
        public decimal TotalAmount => Order.TotalAmount;
        public string RestaurantName => Order.Restaurant?.RestaurantName ?? "Bilinmeyen Restoran";
        public int RestaurantId => Order.RestaurantId;
        public string PaymentMethod => Order.PaymentMethod;
    }
} 