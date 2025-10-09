using cl_backend.DTO;
using cl_backend.Models.Categories;
using cl_backend.Models.Products;

namespace cl_backend.Extensions
{
    namespace cl_backend.Extensions
    {
        public static class MappingExtensions
        {
            // Category to DTO
            public static CategoryDTO ToDTO(this Category category)
            {
                return new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Products = category.Products?.Select(p => p.ToDTO()).ToList() ?? new List<ProductDTO>()
                };
            }

            public static Category ToEntity(this CategoryCreateDTO dto)
            {
                return new Category
                {
                    Name = dto.Name,
                    Description = dto.Description
                };
            }

            public static void UpdateFromDTO(this Category category, CategoryUpdateDTO dto)
            {
                category.Name = dto.Name;
                category.Description = dto.Description;
            }

            // Product to DTO
            public static ProductDTO ToDTO(this Product product)
            {
                return new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    SKU = product.SKU,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name ?? string.Empty,
                    Images = product.Images?.Select(i => i.ToDTO()).ToList() ?? new List<ProductImageDTO>(),
                    Reviews = product.Reviews?.Select(r => r.ToDTO()).ToList() ?? new List<ProductReviewDTO>()
                };
            }

            public static ProductImageDTO ToDTO(this ProductImage image)
            {
                return new ProductImageDTO
                {
                    Id = image.Id,
                    ImageUrl = image.ImageUrl,
                    AltText = image.AltText
                };
            }

            public static ProductReviewDTO ToDTO(this ProductReview review)
            {
                return new ProductReviewDTO
                {
                    Id = review.Id,
                    AuthorName = review.AuthorName,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt
                };
            }
        }
    }
}
