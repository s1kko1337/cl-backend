using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cl_backend.Models.Products
{
    public class ProductImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string ImageUrl { get; set; }
        public string? AltText { get; set; }

        public int ProductId { get; set; }
        public required Product Product { get; set; }
    }
}
