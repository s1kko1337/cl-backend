namespace cl_backend.DTO
{
    // Дневная статистика продаж
    public class DailySalesReportDTO
    {
        public DateTime Date { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ItemsSold { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    // Отчет за период
    public class PeriodSalesReportDTO
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalItemsSold { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<DailySalesReportDTO> DailySales { get; set; } = new List<DailySalesReportDTO>();
    }

    // Помесячная выручка
    public class MonthlyRevenueDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public required string MonthName { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ItemsSold { get; set; }
    }

    // Топ продаваемых товаров
    public class TopProductDTO
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string SKU { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OrdersCount { get; set; }
    }

    // Продажи по категориям
    public class CategorySalesDTO
    {
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public int ProductsCount { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OrdersCount { get; set; }
        public decimal AveragePrice { get; set; }
    }

    // Статистика по способам оплаты
    public class PaymentMethodStatsDTO
    {
        public required string PaymentMethod { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal Percentage { get; set; }
    }
}
