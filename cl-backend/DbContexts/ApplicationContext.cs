using cl_backend.Models.Categories;
using cl_backend.Models.Products;
using cl_backend.Models.Sales;
using cl_backend.Models.User;
using Microsoft.EntityFrameworkCore;

namespace cl_backend.DbContexts
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null;
        public DbSet<Category> Categories { get; set; } = null;
        public DbSet<Product> Products { get; set; } = null;
        public DbSet<ProductImage> ProductImages { get; set; } = null;
        public DbSet<ProductReview> ProductReviews { get; set; } = null;
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public ApplicationContext ()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=cl_backend_db;Username=postgres;Password=210176");
        }
    }
}
