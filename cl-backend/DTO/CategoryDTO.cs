namespace cl_backend.DTO
{
    /// <summary>
    /// DTO категории товаров с полной информацией
    /// </summary>
    public class CategoryDTO
    {
        /// <summary>
        /// Идентификатор категории
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Описание категории
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Список товаров в категории
        /// </summary>
        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
    }
}
