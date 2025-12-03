using cl_backend.Models.Products;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cl_backend.Models.Categories
{
    /// <summary>
    /// Модель категории товаров
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Уникальный идентификатор категории
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Описание категории
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Коллекция товаров принадлежащих данной категории
        /// </summary>
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
