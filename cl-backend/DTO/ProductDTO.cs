namespace cl_backend.DTO
{
    /// <summary>
    /// DTO товара с полной информацией
    /// </summary>
    public class ProductDTO
    {
        /// <summary>
        /// Идентификатор товара
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Описание товара
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Цена товара
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Количество на складе
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Артикул товара (SKU)
        /// </summary>
        public required string SKU { get; set; }

        /// <summary>
        /// Идентификатор категории
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Коллекция изображений товара
        /// </summary>
        public List<ProductImageDTO> Images { get; set; } = new List<ProductImageDTO>();

        /// <summary>
        /// Коллекция отзывов о товаре
        /// </summary>
        public List<ProductReviewDTO> Reviews { get; set; } = new List<ProductReviewDTO>();
    }
}
