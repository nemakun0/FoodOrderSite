using Microsoft.EntityFrameworkCore;

namespace FoodOrderSite.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<UserTable> UserTables { get; set; }
        public DbSet<RestaurantTable> RestaurantTables { get; set; }
        public DbSet<CustomerDeliveryAdderss> CustomerDeliveryAdderss { get; set; }
        public DbSet<FoodItemTable> FoodItemTables { get; set;}
        public DbSet<CategoriesTable> CategoriesTables { get; set; }
        public DbSet<FoodItemCategoriesTable> FoodItemCategoriesTables { get; set; }
        public DbSet<ScheduleTable> Schedules { get; set; }
        public DbSet<RestaurantType> RestaurantTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<ReviewTable> ReviewTable { get; set; }


        // Diğer tablolar da buraya eklenecek
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // FoodItemCategoriesTable için bileşik anahtar tanımı
            modelBuilder.Entity<FoodItemCategoriesTable>()
                .HasKey(f => new { f.FoodItemId, f.CategoryId });

            // İlişkileri de burada tanımlayabilirsiniz (opsiyonel)
            modelBuilder.Entity<FoodItemCategoriesTable>()
                .HasOne(f => f.FoodItem)
                .WithMany()
                .HasForeignKey(f => f.FoodItemId)
                .OnDelete(DeleteBehavior.Cascade); // veya Restrict

            modelBuilder.Entity<FoodItemCategoriesTable>()
                .HasOne(f => f.Category)
                .WithMany()
                .HasForeignKey(f => f.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // veya Restrict

            modelBuilder.Entity<ReviewTable>()
                .HasIndex(r => r.OrderId)
                .IsUnique(); // Her siparişe 1 yorum

            modelBuilder.Entity<ReviewTable>()
            .HasOne(r => r.Restaurant)
            .WithMany()
            .HasForeignKey(r => r.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReviewTable>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReviewTable>()
            .HasOne(r => r.Order)
            .WithMany()
            .HasForeignKey(r => r.OrderId)
            .OnDelete(DeleteBehavior.Cascade);




            // Diğer entity konfigürasyonları...
        }
    }
}
