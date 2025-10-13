using cl_backend.DbContexts;
using cl_backend.DTO;
using cl_backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cl_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/products/{productId}/reviews")]
    public class ProductReviewsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ProductReviewsController(ApplicationContext context)
        {
            _context = context;
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

            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductReviewExists(int id)
        {
            return _context.ProductReviews.Any(e => e.Id == id);
        }
    }
}
