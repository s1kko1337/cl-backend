using cl_backend.DbContexts;
using cl_backend.DTO;
using cl_backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace cl_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/products/{productId}/reviews")]
    public class ProductReviewsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _env;
        private const long MaxFileSize = 12 * 1024 * 1024;
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public ProductReviewsController(ApplicationContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/products/5/reviews - получить все отзывы товара
        [HttpGet]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<IEnumerable<ProductReviewDTO>>> GetProductReviews(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var reviews = await _context.ProductReviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            return Ok(reviews.Select(r => r.ToDTO()));
        }

        // GET: api/products/5/reviews/10 - получить отзыв по ID
        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<ProductReviewDTO>> GetProductReview(int productId, int id)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var review = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.Id == id && r.ProductId == productId);

            if (review == null)
            {
                return NotFound("Review not found.");
            }

            return review.ToDTO();
        }

        // GET: api/products/5/reviews/10/image - получить информацию об изображении отзыва
        [HttpGet("{id}/image")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetReviewImage(int productId, int id)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var review = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.Id == id && r.ProductId == productId);

            if (review == null)
            {
                return NotFound("Review not found.");
            }

            if (string.IsNullOrEmpty(review.ReviewImageUrl))
            {
                return NotFound("Review has no image.");
            }

            return Ok(new
            {
                reviewId = review.Id,
                imageUrl = $"{Request.Scheme}://{Request.Host}{review.ReviewImageUrl}",
                altText = $"Review image for review {review.Id}"
            });
        }

        // GET: api/products/5/reviews/10/image/download - скачать изображение отзыва
        [HttpGet("{id}/image/download")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadReviewImage(int productId, int id)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var review = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.Id == id && r.ProductId == productId);

            if (review == null)
            {
                return NotFound("Review not found.");
            }

            if (string.IsNullOrEmpty(review.ReviewImageUrl))
            {
                return NotFound("Review has no image.");
            }

            try
            {
                var filePath = Path.Combine(_env.ContentRootPath, review.ReviewImageUrl.TrimStart('/'));

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found on server.");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var contentType = GetContentType(filePath);
                var fileName = Path.GetFileName(filePath);

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while downloading the file: {ex.Message}");
            }
        }

        // POST: api/products/5/reviews - добавить отзыв товара
        [HttpPost]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<ProductReviewDTO>> CreateProductReview(int productId, ProductReviewCreateDTO reviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var review = reviewDto.ToEntity(productId);
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductReview), new { productId = productId, id = review.Id }, review.ToDTO());
        }

        // POST: api/products/5/reviews/10/image - загрузить изображение для отзыва
        [HttpPost("{id}/image")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<object>> UploadReviewImage(int productId, int id, IFormFile file)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var review = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.Id == id && r.ProductId == productId);

            if (review == null)
            {
                return NotFound("Review not found.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided.");
            }

            if (file.Length > MaxFileSize)
            {
                return BadRequest($"File size must not exceed {MaxFileSize / (1024 * 1024)}MB.");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(fileExtension))
            {
                return BadRequest($"File extension '{fileExtension}' is not allowed. Allowed extensions: {string.Join(", ", AllowedExtensions)}");
            }

            try
            {
                int userId = review.AuthorId;

                // /uploads/user-{userId}/review-{reviewId}/
                var uploadsDirectory = Path.Combine(_env.ContentRootPath, "uploads", $"user-{userId}", $"review-{id}");
                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/user-{userId}/review-{id}/{fileName}";

                review.ReviewImageUrl = imageUrl;
                review.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Image uploaded successfully",
                    reviewId = review.Id,
                    imageUrl = $"{Request.Scheme}://{Request.Host}{imageUrl}",
                    altText = $"Review image for review {review.Id}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while uploading the file: {ex.Message}");
            }
        }


        // PUT: api/products/5/reviews/10 - обновить отзыв товара
        [HttpPut("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> UpdateProductReview(int productId, int id, ProductReviewUpdateDTO reviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var review = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.Id == id && r.ProductId == productId);

            if (review == null)
            {
                return NotFound("Review not found.");
            }

            review.UpdateFromDTO(reviewDto);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductReviewExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // DELETE: api/products/5/reviews/10 - удалить отзыв товара
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> DeleteProductReview(int productId, int id)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var review = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.Id == id && r.ProductId == productId);

            if (review == null)
            {
                return NotFound("Review not found.");
            }

            try
            {
                // Удаление файла с диска если существует
                if (!string.IsNullOrEmpty(review.ReviewImageUrl))
                {
                    var filePath = Path.Combine(_env.ContentRootPath, review.ReviewImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    // Попытка удалить пустые папки
                    var dirPath = Path.GetDirectoryName(filePath);
                    if (dirPath != null && Directory.Exists(dirPath) && Directory.GetFiles(dirPath).Length == 0)
                    {
                        Directory.Delete(dirPath);
                    }
                }

                _context.ProductReviews.Remove(review);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the review: {ex.Message}");
            }
        }

        // DELETE: api/products/5/reviews/10/image - удалить изображение отзыва
        [HttpDelete("{id}/image")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> DeleteReviewImage(int productId, int id)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var review = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.Id == id && r.ProductId == productId);

            if (review == null)
            {
                return NotFound("Review not found.");
            }

            if (string.IsNullOrEmpty(review.ReviewImageUrl))
            {
                return NotFound("Review has no image.");
            }

            try
            {
                var filePath = Path.Combine(_env.ContentRootPath, review.ReviewImageUrl.TrimStart('/'));

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                review.ReviewImageUrl = null;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the image: {ex.Message}");
            }
        }

        private bool ProductReviewExists(int id)
        {
            return _context.ProductReviews.Any(e => e.Id == id);
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}