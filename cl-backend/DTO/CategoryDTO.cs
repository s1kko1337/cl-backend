namespace cl_backend.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
    }
}
