using cl_backend.DbContexts;
using cl_backend.DTO;
using cl_backend.Extensions;
using cl_backend.Models.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace cl_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/products/{productId}/images")]
    public class ProductImagesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _env;
        private const long MaxFileSize = 12 * 1024 * 1024; // 12MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public ProductImagesController(ApplicationContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/products/5/images - получить все изображения товара
        [HttpGet]
        [Authorize(Roles = "admin, user")]
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
        [Authorize(Roles = "admin, user")]
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

        // GET: api/products/5/images/10/download - скачать конкретное изображение товара
        [HttpGet("{id}/download")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadProductImage(int productId, int id)
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
                var uploadsDirectory = Path.Combine(_env.ContentRootPath, "uploads", $"product-{productId}-images");
                var fileName = Path.GetFileName(image.ImageUrl);
                var filePath = Path.Combine(uploadsDirectory, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found on server.");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var contentType = GetContentType(filePath);

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while downloading the file: {ex.Message}");
            }
        }

        // GET: api/products/5/images/download-all - скачать все изображения товара как ZIP архив
        [HttpGet("download-all")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadAllProductImages(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var images = await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .ToListAsync();

            if (!images.Any())
            {
                return NotFound("No images found for this product.");
            }

            try
            {
                var uploadsDirectory = Path.Combine(_env.ContentRootPath, "uploads", $"product-{productId}-images");
                var tempZipPath = Path.Combine(Path.GetTempPath(), $"product-{productId}-images-{Guid.NewGuid()}.zip");

                // Создание ZIP архива
                using (var zipArchive = System.IO.Compression.ZipFile.Open(tempZipPath, System.IO.Compression.ZipArchiveMode.Create))
                {
                    foreach (var image in images)
                    {
                        var fileName = Path.GetFileName(image.ImageUrl);
                        var filePath = Path.Combine(uploadsDirectory, fileName);

                        if (System.IO.File.Exists(filePath))
                        {
                            zipArchive.CreateEntryFromFile(filePath, fileName);
                        }
                    }
                }

                var zipBytes = await System.IO.File.ReadAllBytesAsync(tempZipPath);

                // Удаление временного файла
                System.IO.File.Delete(tempZipPath);

                return File(zipBytes, "application/zip", $"product-{productId}-images.zip");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the archive: {ex.Message}");
            }
        }

        // GET: api/products/5/images/info - получить информацию о всех изображениях товара (URL + описание)
        [HttpGet("info")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductImageDTO>>> GetProductImagesInfo(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var images = await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .ToListAsync();

            return Ok(images.Select(i => new ProductImageDTO
            {
                Id = i.Id,
                ImageUrl = $"{Request.Scheme}://{Request.Host}/uploads/product-{productId}-images/{Path.GetFileName(i.ImageUrl)}",
                AltText = i.AltText
            }));
        }

        // GET: api/products/5/images/10/info - получить информацию о конкретном изображении (URL + описание)
        [HttpGet("{id}/info")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductImageDTO>> GetProductImageInfo(int productId, int id)
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

            return Ok(new ProductImageDTO
            {
                Id = image.Id,
                ImageUrl = $"{Request.Scheme}://{Request.Host}/uploads/product-{productId}-images/{Path.GetFileName(image.ImageUrl)}",
                AltText = image.AltText
            });
        }

        // POST: api/products/5/images - загрузить изображение товара
        [HttpPost]
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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