using FoodOrderSite.Models;
using System.Collections.Generic;

namespace FoodOrderSite.ViewModels
{
    public class UserManagementViewModel
    {
        public List<UserTable> Customers { get; set; } = new List<UserTable>();
        public List<UserTable> Sellers { get; set; } = new List<UserTable>();
    }
}