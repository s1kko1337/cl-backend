using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    /// <summary>
    /// DTO для обновления данных категории товаров
    /// </summary>
    public class CategoryUpdateDTO
    {
        /// <summary>
        /// Название категории
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public required string Name { get; set; }

        /// <summary>
        /// Описание категории
        /// </summary>
        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
        public string? Description { get; set; }
    }
}
