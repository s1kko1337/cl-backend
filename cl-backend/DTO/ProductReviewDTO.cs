namespace cl_backend.DTO
{
    public class ProductReviewDTO
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public required string AuthorName { get; set; }
        public int Rating { get; set; }
        public required string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? ReviewImageUrl { get; set; }
    }
}
