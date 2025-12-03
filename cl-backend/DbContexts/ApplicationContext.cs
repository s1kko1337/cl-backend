using cl_backend.Models.Categories;
using cl_backend.Models.Products;
using cl_backend.Models.Sales;
using cl_backend.Models.User;
using Microsoft.EntityFrameworkCore;

namespace cl_backend.DbContexts
{
    /// <summary>
    /// Контекст базы данных приложения
    /// </summary>
    public class ApplicationContext : DbContext
    {
        /// <summary>
        /// Таблица пользователей
        /// </summary>
        public DbSet<User> Users { get; set; } = null;

        /// <summary>
        /// Таблица категорий товаров
        /// </summary>
        public DbSet<Category> Categories { get; set; } = null;

        /// <summary>
        /// Таблица товаров
        /// </summary>
        public DbSet<Product> Products { get; set; } = null;

        /// <summary>
        /// Таблица изображений товаров
        /// </summary>
        public DbSet<ProductImage> ProductImages { get; set; } = null;

        /// <summary>
        /// Таблица отзывов о товарах
        /// </summary>
        public DbSet<ProductReview> ProductReviews { get; set; } = null;

        /// <summary>
        /// Таблица заказов
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Таблица элементов заказов
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр контекста базы данных
        /// </summary>
        public ApplicationContext ()
        {

        }

        /// <summary>
        /// Конфигурирует подключение к базе данных PostgreSQL
        /// </summary>
        /// <param name="optionsBuilder">Построитель параметров контекста</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=postgres;Port=5432;Database=cl_backend_db;Username=postgres;Password=210176");
        }
    }
}
