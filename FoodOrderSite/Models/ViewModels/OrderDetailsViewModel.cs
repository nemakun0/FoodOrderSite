using System;
using System.Collections.Generic;
using System.Linq;
using FoodOrderSite.Models;

namespace FoodOrderSite.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Toplam tutarı hesapla (isteğe bağlı, Order.TotalAmount zaten kullanılabilir)
        public decimal TotalAmount => Order?.TotalAmount ?? OrderItems.Sum(item => item.Price * item.Quantity);

        // Sipariş durumu
        public string OrderStatus => Order?.OrderStatus ?? "Beklemede";

        // Sipariş tarihi
        public DateTime OrderDate => Order?.OrderDate ?? DateTime.UtcNow;
    }
}