namespace cl_backend.DTO
{
    public class ProductReviewDTO
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
