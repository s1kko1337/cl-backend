using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    public class ProductCreateDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 200 characters")]
        public required string Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters")]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "SKU must be between 1 and 50 characters")]
        public required string SKU { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than 0")]
        public int CategoryId { get; set; }
    }
}
