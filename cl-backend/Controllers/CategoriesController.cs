using cl_backend.DbContexts;
using cl_backend.DTO;
using cl_backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cl_backend.Controllers
{
    /// <summary>
    /// Контроллер для управления категориями товаров
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера категорий
        /// </summary>
        /// <param name="context">Контекст базы данных приложения</param>
        public CategoriesController(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает список всех категорий с продуктами
        /// </summary>
        /// <returns>Коллекция DTO категорий включая продукты с изображениями и отзывами</returns>
        [HttpGet]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                    .ThenInclude(p => p.Images)
                .Include(c => c.Products)
                    .ThenInclude(p => p.Reviews)
                .ToListAsync();

            return Ok(categories.Select(c => c.ToDTO()));
        }

        /// <summary>
        /// Получает категорию по идентификатору с продуктами
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns>DTO категории с полной информацией о продуктах</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                    .ThenInclude(p => p.Images)
                .Include(c => c.Products)
                    .ThenInclude(p => p.Reviews)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return category.ToDTO();
        }

        /// <summary>
        /// Получает все товары категории
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns>Коллекция DTO товаров категории</returns>
        [Authorize(Roles = "admin, user")]
        [HttpGet("{id}/products")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                    .ThenInclude(p => p.Images)
                .Include(c => c.Products)
                    .ThenInclude(p => p.Reviews)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category.Products.Select(p => p.ToDTO()));
        }

        /// <summary>
        /// Создает новую категорию
        /// </summary>
        /// <param name="categoryDto">DTO с данными для создания категории</param>
        /// <returns>Созданная категория в виде DTO</returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory(CategoryCreateDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = categoryDto.ToEntity();
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var createdCategory = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == category.Id);

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, createdCategory?.ToDTO());
        }

        /// <summary>
        /// Обновляет данные категории
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <param name="categoryDto">DTO с обновленными данными категории</param>
        /// <returns>Результат операции обновления</returns>
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.UpdateFromDTO(categoryDto);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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
        /// Удаляет категорию по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns>Результат операции удаления</returns>
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            if (category.Products.Any())
            {
                return BadRequest("Cannot delete category with existing products. Remove products first.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Проверяет существование категории по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns>True если категория существует, иначе False</returns>
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}