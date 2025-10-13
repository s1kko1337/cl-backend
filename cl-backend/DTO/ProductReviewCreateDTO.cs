using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    public class ProductReviewCreateDTO
    {
        [Required(ErrorMessage = "AuthorName is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "AuthorName must be between 1 and 100 characters")]
        public required string AuthorName { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Comment is required")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 1000 characters")]
        public required string Comment { get; set; }
    }
}
