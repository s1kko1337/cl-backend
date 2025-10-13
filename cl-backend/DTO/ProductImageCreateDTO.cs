using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    public class ProductImageCreateDTO
    {
        [Required(ErrorMessage = "ImageUrl is required")]
        [Url(ErrorMessage = "ImageUrl must be a valid URL")]
        public required string ImageUrl { get; set; }

        [StringLength(200, ErrorMessage = "AltText must not exceed 200 characters")]
        public string? AltText { get; set; }
    }
}
