namespace cl_backend.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public required string SKU { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<ProductImageDTO> Images { get; set; } = new List<ProductImageDTO>();
        public List<ProductReviewDTO> Reviews { get; set; } = new List<ProductReviewDTO>();
    }
}
