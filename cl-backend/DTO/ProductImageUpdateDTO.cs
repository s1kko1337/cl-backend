using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    /// <summary>
    /// DTO для обновления изображения товара
    /// </summary>
    public class ProductImageUpdateDTO
    {
        /// <summary>
        /// URL изображения
        /// </summary>
        [Required(ErrorMessage = "ImageUrl is required")]
        [Url(ErrorMessage = "ImageUrl must be a valid URL")]
        public required string ImageUrl { get; set; }

        /// <summary>
        /// Альтернативный текст для изображения
        /// </summary>
        [StringLength(200, ErrorMessage = "AltText must not exceed 200 characters")]
        public string? AltText { get; set; }
    }
}
