using cl_backend.DbContexts;
using cl_backend.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace cl_backend.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/admin/reports")]
    public class SalesReportsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public SalesReportsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/admin/reports/sales/daily - дневная статистика продаж
        [HttpGet("sales/daily")]
        public async Task<ActionResult<IEnumerable<DailySalesReportDTO>>> GetDailySales(
            [FromQuery] int days = 30)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-days);

            var dailySales = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.Status != "Cancelled")
                .Include(o => o.OrderItems)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new DailySalesReportDTO
                {
                    Date = g.Key,
                    OrdersCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    ItemsSold = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity),
                    AverageOrderValue = g.Average(o => o.TotalAmount)
                })
                .OrderByDescending(d => d.Date)
                .ToListAsync();

            return Ok(dailySales);
        }

        // GET: api/admin/reports/sales/period - отчет за период
        [HttpGet("sales/period")]
        public async Task<ActionResult<PeriodSalesReportDTO>> GetPeriodSales(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var fromDate = (from ?? DateTime.UtcNow.AddMonths(-1)).Date;
            var toDate = (to ?? DateTime.UtcNow).Date.AddDays(1).AddSeconds(-1);

            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status != "Cancelled")
                .Include(o => o.OrderItems)
                .ToListAsync();

            var dailySales = orders
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new DailySalesReportDTO
                {
                    Date = g.Key,
                    OrdersCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    ItemsSold = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity),
                    AverageOrderValue = g.Any() ? g.Average(o => o.TotalAmount) : 0
                })
                .OrderBy(d => d.Date)
                .ToList();

            var report = new PeriodSalesReportDTO
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.TotalAmount),
                TotalItemsSold = orders.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity),
                AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalAmount) : 0,
                DailySales = dailySales
            };

            return Ok(report);
        }

        // GET: api/admin/reports/revenue/monthly - помесячная выручка
        [HttpGet("revenue/monthly")]
        public async Task<ActionResult<IEnumerable<MonthlyRevenueDTO>>> GetMonthlyRevenue(
            [FromQuery] int months = 12)
        {
            var startDate = DateTime.UtcNow.AddMonths(-months).Date;

            var monthlyRevenue = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.Status != "Cancelled")
                .Include(o => o.OrderItems)
                .ToListAsync();

            var grouped = monthlyRevenue
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new MonthlyRevenueDTO
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    OrdersCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    ItemsSold = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity)
                })
                .OrderByDescending(m => m.Year)
                .ThenByDescending(m => m.Month)
                .ToList();

            return Ok(grouped);
        }

        // GET: api/admin/reports/top-products - топ продаваемых товаров
        [HttpGet("top-products")]
        public async Task<ActionResult<IEnumerable<TopProductDTO>>> GetTopProducts(
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

            // Получаем SKU для каждого товара
            var productIds = topProducts.Select(p => p.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.SKU);

            var result = topProducts.Select(p => new TopProductDTO
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                SKU = products.ContainsKey(p.ProductId) ? products[p.ProductId] : "N/A",
                TotalQuantitySold = p.TotalQuantitySold,
                TotalRevenue = p.TotalRevenue,
                OrdersCount = p.OrdersCount
            }).ToList();

            return Ok(result);
        }

        // GET: api/admin/reports/sales-by-category - продажи по категориям
        [HttpGet("sales-by-category")]
        public async Task<ActionResult<IEnumerable<CategorySalesDTO>>> GetSalesByCategory(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var fromDate = (from ?? DateTime.UtcNow.AddMonths(-1)).Date;
            var toDate = (to ?? DateTime.UtcNow).Date.AddDays(1).AddSeconds(-1);

            var categorySales = await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.CreatedAt >= fromDate &&
                            oi.Order.CreatedAt <= toDate &&
                            oi.Order.Status != "Cancelled")
                .Join(_context.Products,
                    oi => oi.ProductId,
                    p => p.Id,
                    (oi, p) => new { OrderItem = oi, Product = p })
                .Join(_context.Categories,
                    x => x.Product.CategoryId,
                    c => c.Id,
                    (x, c) => new { x.OrderItem, x.Product, Category = c })
                .GroupBy(x => new { x.Category.Id, x.Category.Name })
                .Select(g => new
                {
                    CategoryId = g.Key.Id,
                    CategoryName = g.Key.Name,
                    ProductsCount = g.Select(x => x.Product.Id).Distinct().Count(),
                    TotalQuantitySold = g.Sum(x => x.OrderItem.Quantity),
                    TotalRevenue = g.Sum(x => x.OrderItem.Subtotal),
                    OrdersCount = g.Select(x => x.OrderItem.OrderId).Distinct().Count(),
                    AveragePrice = g.Average(x => x.OrderItem.PriceAtPurchase)
                })
                .OrderByDescending(c => c.TotalRevenue)
                .ToListAsync();

            var result = categorySales.Select(c => new CategorySalesDTO
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                ProductsCount = c.ProductsCount,
                TotalQuantitySold = c.TotalQuantitySold,
                TotalRevenue = c.TotalRevenue,
                OrdersCount = c.OrdersCount,
                AveragePrice = c.AveragePrice
            }).ToList();

            return Ok(result);
        }

        // GET: api/admin/reports/payment-methods - распределение по способам оплаты
        [HttpGet("payment-methods")]
        public async Task<ActionResult<IEnumerable<PaymentMethodStatsDTO>>> GetPaymentMethodStats(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var fromDate = (from ?? DateTime.UtcNow.AddMonths(-1)).Date;
            var toDate = (to ?? DateTime.UtcNow).Date.AddDays(1).AddSeconds(-1);

            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= fromDate &&
                           o.CreatedAt <= toDate &&
                           o.Status != "Cancelled")
                .ToListAsync();

            var totalRevenue = orders.Sum(o => o.TotalAmount);

            var paymentStats = orders
                .GroupBy(o => o.PaymentMethod)
                .Select(g => new PaymentMethodStatsDTO
                {
                    PaymentMethod = g.Key,
                    OrdersCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    Percentage = totalRevenue > 0 ? (g.Sum(o => o.TotalAmount) / totalRevenue * 100) : 0
                })
                .OrderByDescending(p => p.TotalRevenue)
                .ToList();

            return Ok(paymentStats);
        }

        // GET: api/admin/reports/dashboard - общая статистика для dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardSummaryDTO>> GetDashboard()
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var weekStart = now.Date.AddDays(-7);
            var monthStart = now.Date.AddMonths(-1);

            // Получаем все заказы (кроме отмененных) для расчетов
            var allOrders = await _context.Orders
                .Where(o => o.Status != "Cancelled")
                .Include(o => o.OrderItems)
                .ToListAsync();

            // Заказы по периодам
            var todayOrders = allOrders.Where(o => o.CreatedAt >= todayStart).ToList();
            var weekOrders = allOrders.Where(o => o.CreatedAt >= weekStart).ToList();
            var monthOrders = allOrders.Where(o => o.CreatedAt >= monthStart).ToList();

            // Общая статистика
            var totalProducts = await _context.Products.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalUsers = await _context.Users.CountAsync();

            // Товары с низким остатком
            var lowStockProducts = await _context.Products.CountAsync(p => p.StockQuantity > 0 && p.StockQuantity <= 10);
            var outOfStockProducts = await _context.Products.CountAsync(p => p.StockQuantity == 0);

            // Заказы по статусам
            var ordersByStatus = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            // Последние 5 заказов
            var recentOrders = await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new RecentOrderDTO
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            var dashboard = new DashboardSummaryDTO
            {
                // Общая статистика
                TotalOrders = allOrders.Count,
                TotalProducts = totalProducts,
                TotalCategories = totalCategories,
                TotalUsers = totalUsers,
                TotalRevenue = allOrders.Sum(o => o.TotalAmount),

                // Сегодня
                TodayOrders = todayOrders.Count,
                TodayRevenue = todayOrders.Sum(o => o.TotalAmount),

                // Неделя
                WeekOrders = weekOrders.Count,
                WeekRevenue = weekOrders.Sum(o => o.TotalAmount),

                // Месяц
                MonthOrders = monthOrders.Count,
                MonthRevenue = monthOrders.Sum(o => o.TotalAmount),

                // KPI
                AverageOrderValue = allOrders.Any() ? allOrders.Average(o => o.TotalAmount) : 0,
                LowStockProductsCount = lowStockProducts,
                OutOfStockProductsCount = outOfStockProducts,
                PendingOrdersCount = ordersByStatus.ContainsKey("Pending") ? ordersByStatus["Pending"] : 0,

                // Статусы заказов
                OrdersByStatus = ordersByStatus,

                // Последние заказы
                RecentOrders = recentOrders
            };

            return Ok(dashboard);
        }

        // GET: api/admin/reports/alerts - алерты для dashboard
        [HttpGet("alerts")]
        public async Task<ActionResult<IEnumerable<DashboardAlertDTO>>> GetAlerts()
        {
            var alerts = new List<DashboardAlertDTO>();

            // Проверяем товары с низким остатком
            var lowStockCount = await _context.Products.CountAsync(p => p.StockQuantity > 0 && p.StockQuantity <= 10);
            if (lowStockCount > 0)
            {
                alerts.Add(new DashboardAlertDTO
                {
                    Type = "warning",
                    Message = "Товары с низким остатком",
                    Count = lowStockCount
                });
            }

            // Проверяем товары, закончившиеся на складе
            var outOfStockCount = await _context.Products.CountAsync(p => p.StockQuantity == 0);
            if (outOfStockCount > 0)
            {
                alerts.Add(new DashboardAlertDTO
                {
                    Type = "error",
                    Message = "Товары закончились на складе",
                    Count = outOfStockCount
                });
            }

            // Проверяем необработанные заказы
            var pendingOrdersCount = await _context.Orders.CountAsync(o => o.Status == "Pending");
            if (pendingOrdersCount > 0)
            {
                alerts.Add(new DashboardAlertDTO
                {
                    Type = "info",
                    Message = "Необработанные заказы",
                    Count = pendingOrdersCount
                });
            }

            return Ok(alerts);
        }
    }
}
