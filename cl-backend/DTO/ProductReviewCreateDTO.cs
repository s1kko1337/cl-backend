using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    /// <summary>
    /// DTO для создания нового отзыва о товаре
    /// </summary>
    public class ProductReviewCreateDTO
    {
        /// <summary>
        /// Идентификатор автора отзыва
        /// </summary>
        [Required(ErrorMessage = "AuthorId is required")]
        public int AuthorId { get; set; }

        /// <summary>
        /// Имя автора отзыва
        /// </summary>
        [Required(ErrorMessage = "AuthorName is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "AuthorName must be between 1 and 100 characters")]
        public required string AuthorName { get; set; }

        /// <summary>
        /// Рейтинг товара от 1 до 5
        /// </summary>
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        /// <summary>
        /// Текст отзыва
        /// </summary>
        [Required(ErrorMessage = "Comment is required")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 1000 characters")]
        public required string Comment { get; set; }
    }
}
