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
    /// <summary>
    /// Контроллер для управления изображениями товаров
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/products/{productId}/images")]
    public class ProductImagesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _env;
        private const long MaxFileSize = 12 * 1024 * 1024;
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        /// <summary>
        /// Инициализирует новый экземпляр контроллера изображений товаров
        /// </summary>
        /// <param name="context">Контекст базы данных приложения</param>
        /// <param name="env">Окружение веб-хоста для работы с файловой системой</param>
        public ProductImagesController(ApplicationContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        /// <summary>
        /// Получает все изображения товара
        /// </summary>
        /// <param name="productId">Идентификатор товара</param>
        /// <returns>Коллекция DTO изображений товара</returns>
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

        /// <summary>
        /// Получает изображение товара по идентификатору
        /// </summary>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="id">Идентификатор изображения</param>
        /// <returns>DTO изображения товара</returns>
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

        /// <summary>
        /// Скачивает файл изображения товара
        /// </summary>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="id">Идентификатор изображения</param>
        /// <returns>Файл изображения</returns>
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

        /// <summary>
        /// Скачивает все изображения товара в виде ZIP архива
        /// </summary>
        /// <param name="productId">Идентификатор товара</param>
        /// <returns>ZIP архив с изображениями товара</returns>
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

                System.IO.File.Delete(tempZipPath);

                return File(zipBytes, "application/zip", $"product-{productId}-images.zip");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the archive: {ex.Message}");
            }
        }

        /// <summary>
        /// Получает информацию о всех изображениях товара с полными URL
        /// </summary>
        /// <param name="productId">Идентификатор товара</param>
        /// <returns>Коллекция DTO изображений с полными URL адресами</returns>
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

        /// <summary>
        /// Получает информацию о конкретном изображении товара с полным URL
        /// </summary>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="id">Идентификатор изображения</param>
        /// <returns>DTO изображения с полным URL адресом</returns>
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

        /// <summary>
        /// Загружает новое изображение для товара
        /// </summary>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="file">Файл изображения для загрузки</param>
        /// <param name="altText">Альтернативный текст для изображения</param>
        /// <returns>Созданное изображение в виде DTO</returns>
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
                var uploadsDirectory = Path.Combine(_env.ContentRootPath, "uploads", $"product-{productId}-images");
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

                var imageUrl = $"/uploads/product-{productId}-images/{fileName}";

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

        /// <summary>
        /// Обновляет метаданные изображения товара
        /// </summary>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="id">Идентификатор изображения</param>
        /// <param name="imageDto">DTO с обновленными данными изображения</param>
        /// <returns>Результат операции обновления</returns>
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

        /// <summary>
        /// Удаляет изображение товара
        /// </summary>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="id">Идентификатор изображения</param>
        /// <returns>Результат операции удаления</returns>
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
                var uploadsDirectory = Path.Combine(_env.ContentRootPath, "uploads", $"product-{productId}-images");
                var fileName = Path.GetFileName(image.ImageUrl);
                var filePath = Path.Combine(uploadsDirectory, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the image: {ex.Message}");
            }
        }

        /// <summary>
        /// Проверяет существование изображения по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор изображения</param>
        /// <returns>True если изображение существует, иначе False</returns>
        private bool ProductImageExists(int id)
        {
            return _context.ProductImages.Any(e => e.Id == id);
        }

        /// <summary>
        /// Определяет MIME тип файла по его расширению
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <returns>MIME тип контента</returns>
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