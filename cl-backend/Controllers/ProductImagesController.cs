using cl_backend.DbContexts;
using cl_backend.DTO;
using cl_backend.Extensions;
using cl_backend.Models.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cl_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/products/{productId}/images")]
    public class ProductImagesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _env;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public ProductImagesController(ApplicationContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/products/5/images - получить все изображения товара
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductImageDTO>>> GetProductImages(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var images = await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .ToListAsync();

            return Ok(images.Select(i => i.ToDTO()));
        }

        // GET: api/products/5/images/10 - получить изображение по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductImageDTO>> GetProductImage(int productId, int id)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var image = await _context.ProductImages
                .FirstOrDefaultAsync(i => i.Id == id && i.ProductId == productId);

            if (image == null)
            {
                return NotFound("Image not found.");
            }

            return image.ToDTO();
        }

        // POST: api/products/5/images - загрузить изображение товара
        [HttpPost]
        public async Task<ActionResult<ProductImageDTO>> UploadProductImage(int productId, IFormFile file, [FromForm] string? altText)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided.");
            }

            // Проверка размера файла
            if (file.Length > MaxFileSize)
            {
                return BadRequest($"File size must not exceed {MaxFileSize / (1024 * 1024)}MB.");
            }

            // Проверка расширения файла
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(fileExtension))
            {
                return BadRequest($"File extension '{fileExtension}' is not allowed. Allowed extensions: {string.Join(", ", AllowedExtensions)}");
            }

            try
            {
                // Создание папки для товара
                var uploadsDirectory = Path.Combine(_env.ContentRootPath, "uploads", $"product-{productId}-images");
                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                // Генерирование уникального имени файла
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsDirectory, fileName);

                // Сохранение файла
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Формирование URL изображения
                var imageUrl = $"/uploads/product-{productId}-images/{fileName}";

                // Создание записи в БД
                var image = new ProductImage
                {
                    ImageUrl = imageUrl,
                    AltText = altText ?? file.FileName,
                    ProductId = productId
                };

                _context.ProductImages.Add(image);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProductImage), new { productId = productId, id = image.Id }, image.ToDTO());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while uploading the file: {ex.Message}");
            }
        }

        // PUT: api/products/5/images/10 - обновить данные изображения (только altText)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductImage(int productId, int id, [FromBody] ProductImageUpdateDTO imageDto)
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

            var image = await _context.ProductImages
                .FirstOrDefaultAsync(i => i.Id == id && i.ProductId == productId);

            if (image == null)
            {
                return NotFound("Image not found.");
            }

            image.UpdateFromDTO(imageDto);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductImageExists(id))
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

        // DELETE: api/products/5/images/10 - удалить изображение товара
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductImage(int productId, int id)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var image = await _context.ProductImages
                .FirstOrDefaultAsync(i => i.Id == id && i.ProductId == productId);

            if (image == null)
            {
                return NotFound("Image not found.");
            }

            try
            {
                // Удаление файла с диска
                var uploadsDirectory = Path.Combine(_env.ContentRootPath, "uploads", $"product-{productId}-images");
                var fileName = Path.GetFileName(image.ImageUrl);
                var filePath = Path.Combine(uploadsDirectory, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Удаление записи из БД
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the image: {ex.Message}");
            }
        }

        private bool ProductImageExists(int id)
        {
            return _context.ProductImages.Any(e => e.Id == id);
        }
    }
}