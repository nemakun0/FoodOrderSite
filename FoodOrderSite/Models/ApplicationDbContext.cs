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
        public DbSet<FoodItemTable> FoodItemTables { get; set;}
        public DbSet<CategoriesTable> CategoriesTables { get; set; }
        public DbSet<FoodItemCategoriesTable> FoodItemCategoriesTables { get; set; }
        public DbSet<ScheduleTable> Schedules { get; set; }

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

            // Diğer entity konfigürasyonları...
        }
    }
}
