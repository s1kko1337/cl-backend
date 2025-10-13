using cl_backend.DTO;
using cl_backend.Models.Categories;
using cl_backend.Models.Products;
using cl_backend.Models.User;

namespace cl_backend.Extensions
{
    public static class MappingExtensions
    {
        #region Category Mapping

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

        #endregion

        #region Product Mapping

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

        public static Product ToEntity(this ProductCreateDTO dto)
        {
            return new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                SKU = dto.SKU,
                CategoryId = dto.CategoryId
            };
        }

        public static void UpdateFromDTO(this Product product, ProductUpdateDTO dto)
        {
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.SKU = dto.SKU;
            product.CategoryId = dto.CategoryId;
        }

        #endregion

        #region ProductImage Mapping

        public static ProductImageDTO ToDTO(this ProductImage image)
        {
            return new ProductImageDTO
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl,
                AltText = image.AltText
            };
        }

        public static ProductImage ToEntity(this ProductImageCreateDTO dto, int productId)
        {
            return new ProductImage
            {
                ImageUrl = dto.ImageUrl,
                AltText = dto.AltText,
                ProductId = productId
            };
        }

        public static void UpdateFromDTO(this ProductImage image, ProductImageUpdateDTO dto)
        {
            image.ImageUrl = dto.ImageUrl;
            image.AltText = dto.AltText;
        }

        #endregion

        #region ProductReview Mapping

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

        public static ProductReview ToEntity(this ProductReviewCreateDTO dto, int productId)
        {
            return new ProductReview
            {
                AuthorName = dto.AuthorName,
                Rating = dto.Rating,
                Comment = dto.Comment,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateFromDTO(this ProductReview review, ProductReviewUpdateDTO dto)
        {
            review.AuthorName = dto.AuthorName;
            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
        }

        #endregion

        #region User Mapping

        public static UserDTO ToDTO(this User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Login = user.Login,
                Role = user.Role
            };
        }

        public static User ToEntity(this RegisterRequest dto)
        {
            return new User
            {
                Login = dto.Username,
                Password = dto.Password,
                Role = "user" // Default role for new users
            };
        }

        #endregion
    }
}