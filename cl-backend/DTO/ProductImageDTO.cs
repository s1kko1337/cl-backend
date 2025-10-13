namespace cl_backend.DTO
{
    public class ProductImageDTO
    {
        public int Id { get; set; }
        public required string ImageUrl { get; set; }
        public string? AltText { get; set; }
    }
}
