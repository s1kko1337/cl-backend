namespace cl_backend.DTO
{
    /// <summary>
    /// DTO изображения товара
    /// </summary>
    public class ProductImageDTO
    {
        /// <summary>
        /// Идентификатор изображения
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// URL изображения
        /// </summary>
        public required string ImageUrl { get; set; }

        /// <summary>
        /// Альтернативный текст для изображения
        /// </summary>
        public string? AltText { get; set; }
    }
}
