using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using cl_backend.Models.User;

namespace cl_backend.Models.Products
{
    /// <summary>
    /// Модель отзыва о товаре
    /// </summary>
    public class ProductReview
    {
        /// <summary>
        /// Уникальный идентификатор отзыва
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор автора отзыва
        /// </summary>
        public int AuthorId { get; set; }

        /// <summary>
        /// Имя автора отзыва
        /// </summary>
        public required string AuthorName { get; set; }

        /// <summary>
        /// Рейтинг товара от 1 до 5
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Текст отзыва
        /// </summary>
        public required string Comment { get; set; }

        /// <summary>
        /// Дата и время создания отзыва
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Идентификатор товара к которому относится отзыв
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Дата и время последнего обновления отзыва
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// URL изображения прикрепленного к отзыву
        /// </summary>
        public string? ReviewImageUrl { get; set; }
    }
}
