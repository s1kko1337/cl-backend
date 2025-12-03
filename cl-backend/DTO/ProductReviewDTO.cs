namespace cl_backend.DTO
{
    /// <summary>
    /// DTO отзыва о товаре с полной информацией
    /// </summary>
    public class ProductReviewDTO
    {
        /// <summary>
        /// Идентификатор отзыва
        /// </summary>
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
        /// Дата создания отзыва
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата последнего обновления отзыва
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// URL изображения к отзыву
        /// </summary>
        public string? ReviewImageUrl { get; set; }
    }
}
