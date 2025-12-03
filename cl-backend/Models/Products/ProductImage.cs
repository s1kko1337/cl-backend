using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cl_backend.Models.Products
{
    /// <summary>
    /// Модель изображения товара
    /// </summary>
    public class ProductImage
    {
        /// <summary>
        /// Уникальный идентификатор изображения
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// URL путь к файлу изображения
        /// </summary>
        public required string ImageUrl { get; set; }

        /// <summary>
        /// Альтернативный текст для изображения
        /// </summary>
        public string? AltText { get; set; }

        /// <summary>
        /// Идентификатор товара к которому принадлежит изображение
        /// </summary>
        public int ProductId { get; set; }
    }
}
