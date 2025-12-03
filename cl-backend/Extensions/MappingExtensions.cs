using cl_backend.DTO;
using cl_backend.Models.Categories;
using cl_backend.Models.Products;
using cl_backend.Models.Sales;
using cl_backend.Models.User;

namespace cl_backend.Extensions
{
    /// <summary>
    /// Методы расширения для маппинга между моделями и DTO
    /// </summary>
    public static class MappingExtensions
    {
        /// <summary>
        /// Преобразует модель Category в CategoryDTO
        /// </summary>
        /// <param name="category">Модель категории</param>
        /// <returns>DTO категории с полной информацией</returns>
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

        /// <summary>
        /// Преобразует CategoryCreateDTO в модель Category
        /// </summary>
        /// <param name="dto">DTO для создания категории</param>
        /// <returns>Модель категории</returns>
        public static Category ToEntity(this CategoryCreateDTO dto)
        {
            return new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };
        }

        /// <summary>
        /// Обновляет модель Category данными из CategoryUpdateDTO
        /// </summary>
        /// <param name="category">Модель категории для обновления</param>
        /// <param name="dto">DTO с новыми данными категории</param>
        public static void UpdateFromDTO(this Category category, CategoryUpdateDTO dto)
        {
            category.Name = dto.Name;
            category.Description = dto.Description;
        }

        /// <summary>
        /// Преобразует модель Product в ProductDTO
        /// </summary>
        /// <param name="product">Модель товара</param>
        /// <returns>DTO товара с полной информацией включая категорию, изображения и отзывы</returns>
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

        /// <summary>
        /// Преобразует ProductCreateDTO в модель Product
        /// </summary>
        /// <param name="dto">DTO для создания товара</param>
        /// <returns>Модель товара</returns>
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

        /// <summary>
        /// Обновляет модель Product данными из ProductUpdateDTO
        /// </summary>
        /// <param name="product">Модель товара для обновления</param>
        /// <param name="dto">DTO с новыми данными товара</param>
        public static void UpdateFromDTO(this Product product, ProductUpdateDTO dto)
        {
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.SKU = dto.SKU;
            product.CategoryId = dto.CategoryId;
        }

        /// <summary>
        /// Преобразует модель ProductImage в ProductImageDTO
        /// </summary>
        /// <param name="image">Модель изображения товара</param>
        /// <returns>DTO изображения товара</returns>
        public static ProductImageDTO ToDTO(this ProductImage image)
        {
            return new ProductImageDTO
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl,
                AltText = image.AltText
            };
        }

        /// <summary>
        /// Преобразует ProductImageCreateDTO в модель ProductImage
        /// </summary>
        /// <param name="dto">DTO для создания изображения</param>
        /// <param name="productId">Идентификатор товара, к которому относится изображение</param>
        /// <returns>Модель изображения товара</returns>
        public static ProductImage ToEntity(this ProductImageCreateDTO dto, int productId)
        {
            return new ProductImage
            {
                ImageUrl = dto.ImageUrl,
                AltText = dto.AltText,
                ProductId = productId
            };
        }

        /// <summary>
        /// Обновляет модель ProductImage данными из ProductImageUpdateDTO
        /// </summary>
        /// <param name="image">Модель изображения для обновления</param>
        /// <param name="dto">DTO с новыми данными изображения</param>
        public static void UpdateFromDTO(this ProductImage image, ProductImageUpdateDTO dto)
        {
            image.ImageUrl = dto.ImageUrl;
            image.AltText = dto.AltText;
        }

        /// <summary>
        /// Преобразует модель ProductReview в ProductReviewDTO
        /// </summary>
        /// <param name="review">Модель отзыва</param>
        /// <returns>DTO отзыва с полной информацией</returns>
        public static ProductReviewDTO ToDTO(this ProductReview review)
        {
            return new ProductReviewDTO
            {
                Id = review.Id,
                AuthorId = review.AuthorId,
                AuthorName = review.AuthorName,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                ReviewImageUrl = review.ReviewImageUrl
            };
        }

        /// <summary>
        /// Преобразует ProductReviewCreateDTO в модель ProductReview
        /// </summary>
        /// <param name="dto">DTO для создания отзыва</param>
        /// <param name="productId">Идентификатор товара, к которому относится отзыв</param>
        /// <returns>Модель отзыва с установленной датой создания</returns>
        public static ProductReview ToEntity(this ProductReviewCreateDTO dto, int productId)
        {
            return new ProductReview
            {
                AuthorId = dto.AuthorId,
                AuthorName = dto.AuthorName,
                Rating = dto.Rating,
                Comment = dto.Comment,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Обновляет модель ProductReview данными из ProductReviewUpdateDTO
        /// </summary>
        /// <param name="review">Модель отзыва для обновления</param>
        /// <param name="dto">DTO с новыми данными отзыва</param>
        public static void UpdateFromDTO(this ProductReview review, ProductReviewUpdateDTO dto)
        {
            review.AuthorName = dto.AuthorName;
            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Преобразует модель User в UserDTO
        /// </summary>
        /// <param name="user">Модель пользователя</param>
        /// <returns>DTO пользователя для передачи клиенту</returns>
        public static UserDTO ToDTO(this User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Login = user.Login,
                Username = user.Username,
                Role = user.Role
            };
        }

        /// <summary>
        /// Преобразует RegisterRequest в модель User
        /// </summary>
        /// <param name="dto">DTO запроса регистрации</param>
        /// <returns>Модель пользователя с ролью по умолчанию "user"</returns>
        public static User ToEntity(this RegisterRequest dto)
        {
            return new User
            {
                Login = dto.Email,
                Username = dto.Username,
                Password = dto.Password,
                Role = "user"
            };
        }

        /// <summary>
        /// Преобразует модель Order в OrderDTO
        /// </summary>
        /// <param name="order">Модель заказа</param>
        /// <returns>DTO заказа с полной информацией и элементами заказа</returns>
        public static OrderDTO ToDTO(this Order order)
        {
            return new OrderDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                DeliveryAddress = order.DeliveryAddress,
                PaymentMethod = order.PaymentMethod,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                OrderItems = order.OrderItems?.Select(oi => oi.ToDTO()).ToList() ?? new List<OrderItemDTO>()
            };
        }

        /// <summary>
        /// Преобразует модель OrderItem в OrderItemDTO
        /// </summary>
        /// <param name="orderItem">Модель элемента заказа</param>
        /// <returns>DTO элемента заказа с информацией о товаре</returns>
        public static OrderItemDTO ToDTO(this OrderItem orderItem)
        {
            return new OrderItemDTO
            {
                Id = orderItem.Id,
                ProductId = orderItem.ProductId,
                ProductName = orderItem.ProductName,
                PriceAtPurchase = orderItem.PriceAtPurchase,
                Quantity = orderItem.Quantity,
                Subtotal = orderItem.Subtotal
            };
        }

        /// <summary>
        /// Обновляет модель Order данными из OrderUpdateDTO
        /// </summary>
        /// <param name="order">Модель заказа для обновления</param>
        /// <param name="dto">DTO с новым статусом заказа</param>
        public static void UpdateFromDTO(this Order order, OrderUpdateDTO dto)
        {
            order.Status = dto.Status;
            order.UpdatedAt = DateTime.UtcNow;
        }
    }
}
