using cl_backend.Models.Categories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cl_backend.Models.Products
{
    /// <summary>
    /// Модель товара в каталоге
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Уникальный идентификатор товара
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Описание товара
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Цена товара
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Количество товара на складе
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Артикул товара (уникальный идентификатор SKU)
        /// </summary>
        public required string SKU { get; set; }

        /// <summary>
        /// Идентификатор категории к которой принадлежит товар
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Навигационное свойство категории товара
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Коллекция изображений товара
        /// </summary>
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();

        /// <summary>
        /// Коллекция отзывов о товаре
        /// </summary>
        public List<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    }
}
