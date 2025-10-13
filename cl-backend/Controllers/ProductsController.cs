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
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ProductsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/products - получить все товары
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .ToListAsync();

            return Ok(products.Select(p => p.ToDTO()));
        }

        // GET: api/products/5 - получить товар по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return product.ToDTO();
        }

        // POST: api/products - создать новый товар
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct(ProductCreateDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверяем, существует ли категория
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

            // Проверяем уникальность SKU
            var skuExists = await _context.Products.AnyAsync(p => p.SKU == productDto.SKU);
            if (skuExists)
            {
                return BadRequest("Product with this SKU already exists.");
            }

            var product = productDto.ToEntity();
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var createdProduct = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, createdProduct?.ToDTO());
        }

        // PUT: api/products/5 - обновить товар
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Проверяем, существует ли категория
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

            // Проверяем уникальность SKU (если SKU изменился)
            if (product.SKU != productDto.SKU)
            {
                var skuExists = await _context.Products.AnyAsync(p => p.SKU == productDto.SKU && p.Id != id);
                if (skuExists)
                {
                    return BadRequest("Product with this SKU already exists.");
                }
            }

            product.UpdateFromDTO(productDto);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // DELETE: api/products/5 - удалить товар
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Проверяем, есть ли отзывы
            if (product.Reviews.Any())
            {
                return BadRequest("Cannot delete product with existing reviews. Remove reviews first.");
            }

            // Проверяем, есть ли изображения
            if (product.Images.Any())
            {
                return BadRequest("Cannot delete product with existing images. Remove images first.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}