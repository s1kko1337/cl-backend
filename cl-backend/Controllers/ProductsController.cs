using cl_backend.DbContexts;
using cl_backend.DTO;
using cl_backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cl_backend.Controllers
{
    /// <summary>
    /// Контроллер для управления товарами
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера товаров
        /// </summary>
        /// <param name="context">Контекст базы данных приложения</param>
        public ProductsController(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает список всех товаров с полной информацией
        /// </summary>
        /// <returns>Коллекция DTO товаров включая категории, изображения и отзывы</returns>
        [HttpGet]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .ToListAsync();

            return Ok(products.Select(p => p.ToDTO()));
        }

        /// <summary>
        /// Получает товар по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <returns>DTO товара с полной информацией включая категорию, изображения и отзывы</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")]
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

        /// <summary>
        /// Создает новый товар
        /// </summary>
        /// <param name="productDto">DTO с данными для создания товара</param>
        /// <returns>Созданный товар в виде DTO</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ProductDTO>> CreateProduct(ProductCreateDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

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

        /// <summary>
        /// Обновляет данные товара
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <param name="productDto">DTO с обновленными данными товара</param>
        /// <returns>Результат операции обновления</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
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

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

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

        /// <summary>
        /// Удаляет товар по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <returns>Результат операции удаления</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
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

            if (product.Reviews.Any())
            {
                return BadRequest("Cannot delete product with existing reviews. Remove reviews first.");
            }

            if (product.Images.Any())
            {
                return BadRequest("Cannot delete product with existing images. Remove images first.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Проверяет существование товара по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <returns>True если товар существует, иначе False</returns>
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}