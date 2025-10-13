using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    public class CategoryCreateDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
        public required string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
        public string? Description { get; set; }
    }
}
