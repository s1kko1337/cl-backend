namespace cl_backend.DTO
{
    public class ProductImageDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
    }
}
