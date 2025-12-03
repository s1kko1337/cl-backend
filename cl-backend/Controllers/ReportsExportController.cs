using cl_backend.DbContexts;
using cl_backend.DTO;
using cl_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace cl_backend.Controllers
{
    /// <summary>
    /// Контроллер для экспорта отчетов в различные форматы
    /// </summary>
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/admin/export")]
    public class ReportsExportController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IExportService _exportService;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера экспорта отчетов
        /// </summary>
        /// <param name="context">Контекст базы данных приложения</param>
        /// <param name="exportService">Сервис для экспорта данных в разные форматы</param>
        public ReportsExportController(ApplicationContext context, IExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        /// <summary>
        /// Экспортирует список товаров в CSV файл
        /// </summary>
        /// <returns>CSV файл со списком товаров</returns>
        [HttpGet("products/csv")]
        public async Task<IActionResult> ExportProductsToCsv()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.SKU,
                    Category = p.Category.Name,
                    p.Price,
                    p.StockQuantity,
                    p.Description
                })
                .ToListAsync();

            var csvData = _exportService.ExportToCsv(products);
            return File(csvData, "text/csv", $"products_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        /// <summary>
        /// Экспортирует список товаров в Excel файл
        /// </summary>
        /// <returns>Excel файл со списком товаров</returns>
        [HttpGet("products/excel")]
        public async Task<IActionResult> ExportProductsToExcel()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    SKU = p.SKU,
                    Category = p.Category.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Description = p.Description
                })
                .ToListAsync();

            var excelData = _exportService.ExportToExcel(products, "Products");
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"products_{DateTime.UtcNow:yyyyMMdd}.xlsx");
        }

        /// <summary>
        /// Экспортирует список заказов в CSV файл за указанный период
        /// </summary>
        /// <param name="from">Начальная дата периода</param>
        /// <param name="to">Конечная дата периода</param>
        /// <returns>CSV файл со списком заказов</returns>
        [HttpGet("orders/csv")]
        public async Task<IActionResult> ExportOrdersToCsv(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var fromDate = (from ?? DateTime.UtcNow.AddMonths(-1)).Date;
            var toDate = (to ?? DateTime.UtcNow).Date.AddDays(1).AddSeconds(-1);

            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
                .Select(o => new
                {
                    o.Id,
                    o.CustomerName,
                    o.CustomerPhone,
                    o.DeliveryAddress,
                    o.PaymentMethod,
                    o.TotalAmount,
                    o.Status,
                    CreatedAt = o.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync();

            var csvData = _exportService.ExportToCsv(orders);
            return File(csvData, "text/csv", $"orders_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        /// <summary>
        /// Экспортирует список заказов в Excel файл за указанный период
        /// </summary>
        /// <param name="from">Начальная дата периода</param>
        /// <param name="to">Конечная дата периода</param>
        /// <returns>Excel файл со списком заказов</returns>
        [HttpGet("orders/excel")]
        public async Task<IActionResult> ExportOrdersToExcel(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var fromDate = (from ?? DateTime.UtcNow.AddMonths(-1)).Date;
            var toDate = (to ?? DateTime.UtcNow).Date.AddDays(1).AddSeconds(-1);

            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
                .Select(o => new
                {
                    OrderId = o.Id,
                    Customer = o.CustomerName,
                    Phone = o.CustomerPhone,
                    Address = o.DeliveryAddress,
                    PaymentMethod = o.PaymentMethod,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync();

            var excelData = _exportService.ExportToExcel(orders, "Orders");
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"orders_{DateTime.UtcNow:yyyyMMdd}.xlsx");
        }

        /// <summary>
        /// Экспортирует отчет по продажам в CSV файл за указанный период
        /// </summary>
        /// <param name="from">Начальная дата периода</param>
        /// <param name="to">Конечная дата периода</param>
        /// <returns>CSV файл с отчетом по продажам</returns>
        [HttpGet("sales-report/csv")]
        public async Task<IActionResult> ExportSalesReportToCsv(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var fromDate = (from ?? DateTime.UtcNow.AddMonths(-1)).Date;
            var toDate = (to ?? DateTime.UtcNow).Date.AddDays(1).AddSeconds(-1);

            var dailySales = await _context.Orders
                .Where(o => o.CreatedAt >= fromDate &&
                           o.CreatedAt <= toDate &&
                           o.Status != "Cancelled")
                .Include(o => o.OrderItems)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    OrdersCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    ItemsSold = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity),
                    AverageOrderValue = g.Average(o => o.TotalAmount)
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            var csvData = _exportService.ExportToCsv(dailySales);
            return File(csvData, "text/csv", $"sales_report_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        /// <summary>
        /// Экспортирует отчет по продажам в Excel файл за указанный период
        /// </summary>
        /// <param name="from">Начальная дата периода</param>
        /// <param name="to">Конечная дата периода</param>
        /// <returns>Excel файл с отчетом по продажам</returns>
        [HttpGet("sales-report/excel")]
        public async Task<IActionResult> ExportSalesReportToExcel(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var fromDate = (from ?? DateTime.UtcNow.AddMonths(-1)).Date;
            var toDate = (to ?? DateTime.UtcNow).Date.AddDays(1).AddSeconds(-1);

            var dailySales = await _context.Orders
                .Where(o => o.CreatedAt >= fromDate &&
                           o.CreatedAt <= toDate &&
                           o.Status != "Cancelled")
                .Include(o => o.OrderItems)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    OrdersCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    ItemsSold = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity),
                    AverageOrderValue = g.Average(o => o.TotalAmount)
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            var excelData = _exportService.ExportToExcel(dailySales, "Sales Report");
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"sales_report_{DateTime.UtcNow:yyyyMMdd}.xlsx");
        }

        /// <summary>
        /// Экспортирует топ товаров в CSV файл за указанный период
        /// </summary>
        /// <param name="limit">Количество товаров в топе</param>
        /// <param name="from">Начальная дата периода</param>
        /// <param name="to">Конечная дата периода</param>
        /// <returns>CSV файл с топ товарами</returns>
        [HttpGet("top-products/csv")]
        public async Task<IActionResult> ExportTopProductsToCsv(
            [FromQuery] int limit = 10,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var fromDate = (from ?? DateTime.UtcNow.AddMonths(-1)).Date;
            var toDate = (to ?? DateTime.UtcNow).Date.AddDays(1).AddSeconds(-1);

            var topProducts = await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.CreatedAt >= fromDate &&
                            oi.Order.CreatedAt <= toDate &&
                            oi.Order.Status != "Cancelled")
                .GroupBy(oi => new { oi.ProductId, oi.ProductName })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Subtotal),
                    OrdersCount = g.Select(oi => oi.OrderId).Distinct().Count()
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(limit)
                .ToListAsync();

            var csvData = _exportService.ExportToCsv(topProducts);
            return File(csvData, "text/csv", $"top_products_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        /// <summary>
        /// Экспортирует топ товаров в Excel файл за указанный период
        /// </summary>
        /// <param name="limit">Количество товаров в топе</param>
        /// <param name="from">Начальная дата периода</param>
        /// <param name="to">Конечная дата периода</param>
        /// <returns>Excel файл с топ товарами</returns>
        [HttpGet("top-products/excel")]
        public async Task<IActionResult> ExportTopProductsToExcel(
            [FromQuery] int limit = 10,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var fromDate = (from ?? DateTime.UtcNow.AddMonths(-1)).Date;
            var toDate = (to ?? DateTime.UtcNow).Date.AddDays(1).AddSeconds(-1);

            var topProducts = await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.CreatedAt >= fromDate &&
                            oi.Order.CreatedAt <= toDate &&
                            oi.Order.Status != "Cancelled")
                .GroupBy(oi => new { oi.ProductId, oi.ProductName })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Subtotal),
                    OrdersCount = g.Select(oi => oi.OrderId).Distinct().Count()
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(limit)
                .ToListAsync();

            var excelData = _exportService.ExportToExcel(topProducts, "Top Products");
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"top_products_{DateTime.UtcNow:yyyyMMdd}.xlsx");
        }

        /// <summary>
        /// Экспортирует складские остатки в CSV файл
        /// </summary>
        /// <returns>CSV файл со складскими остатками</returns>
        [HttpGet("inventory/csv")]
        public async Task<IActionResult> ExportInventoryToCsv()
        {
            var inventory = await _context.Products
                .Include(p => p.Category)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.SKU,
                    Category = p.Category.Name,
                    p.Price,
                    StockQuantity = p.StockQuantity,
                    TotalValue = p.Price * p.StockQuantity,
                    Status = p.StockQuantity == 0 ? "Out of Stock" :
                            p.StockQuantity <= 10 ? "Low Stock" : "In Stock"
                })
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();

            var csvData = _exportService.ExportToCsv(inventory);
            return File(csvData, "text/csv", $"inventory_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        /// <summary>
        /// Экспортирует складские остатки в Excel файл
        /// </summary>
        /// <returns>Excel файл со складскими остатками</returns>
        [HttpGet("inventory/excel")]
        public async Task<IActionResult> ExportInventoryToExcel()
        {
            var inventory = await _context.Products
                .Include(p => p.Category)
                .Select(p => new
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    SKU = p.SKU,
                    Category = p.Category.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    TotalValue = p.Price * p.StockQuantity,
                    Status = p.StockQuantity == 0 ? "Out of Stock" :
                            p.StockQuantity <= 10 ? "Low Stock" : "In Stock"
                })
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();

            var excelData = _exportService.ExportToExcel(inventory, "Inventory");
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"inventory_{DateTime.UtcNow:yyyyMMdd}.xlsx");
        }
    }
}
