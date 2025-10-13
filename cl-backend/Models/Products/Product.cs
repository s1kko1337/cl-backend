using cl_backend.Models.Categories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cl_backend.Models.Products
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; } 
        public decimal Price { get; set; }
        public int StockQuantity { get; set; } 
        public required string SKU { get; set; } // Артикул товара (уникальный идентификатор)
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
        public List<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    }
}
