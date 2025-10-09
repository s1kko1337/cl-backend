namespace cl_backend.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
    }
}
