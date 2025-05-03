using Microsoft.EntityFrameworkCore;

namespace FoodOrderSite.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserTable> UserTables { get; set; }
        public DbSet<RestaurantTable> RestaurantTables { get; set; }
        public DbSet<CustomerDeliveryAdderss> CustomerDeliveryAdderss { get; set; }

        // Diğer tablolar da buraya eklenecek
    }
}
