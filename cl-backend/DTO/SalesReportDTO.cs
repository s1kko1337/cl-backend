namespace cl_backend.DTO
{
    /// <summary>
    /// DTO дневной статистики продаж
    /// </summary>
    public class DailySalesReportDTO
    {
        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Количество заказов
        /// </summary>
        public int OrdersCount { get; set; }

        /// <summary>
        /// Общая выручка
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Количество проданных товаров
        /// </summary>
        public int ItemsSold { get; set; }

        /// <summary>
        /// Средняя стоимость заказа
        /// </summary>
        public decimal AverageOrderValue { get; set; }
    }

    /// <summary>
    /// DTO отчета за период
    /// </summary>
    public class PeriodSalesReportDTO
    {
        /// <summary>
        /// Начальная дата периода
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Конечная дата периода
        /// </summary>
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Общее количество заказов за период
        /// </summary>
        public int TotalOrders { get; set; }

        /// <summary>
        /// Общая выручка за период
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Общее количество проданных товаров
        /// </summary>
        public int TotalItemsSold { get; set; }

        /// <summary>
        /// Средняя стоимость заказа
        /// </summary>
        public decimal AverageOrderValue { get; set; }

        /// <summary>
        /// Дневная статистика продаж
        /// </summary>
        public List<DailySalesReportDTO> DailySales { get; set; } = new List<DailySalesReportDTO>();
    }

    /// <summary>
    /// DTO помесячной выручки
    /// </summary>
    public class MonthlyRevenueDTO
    {
        /// <summary>
        /// Год
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Номер месяца
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Название месяца
        /// </summary>
        public required string MonthName { get; set; }

        /// <summary>
        /// Количество заказов за месяц
        /// </summary>
        public int OrdersCount { get; set; }

        /// <summary>
        /// Общая выручка за месяц
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Количество проданных товаров
        /// </summary>
        public int ItemsSold { get; set; }
    }

    /// <summary>
    /// DTO топ продаваемых товаров
    /// </summary>
    public class TopProductDTO
    {
        /// <summary>
        /// Идентификатор товара
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public required string ProductName { get; set; }

        /// <summary>
        /// Артикул товара
        /// </summary>
        public required string SKU { get; set; }

        /// <summary>
        /// Общее количество проданных единиц
        /// </summary>
        public int TotalQuantitySold { get; set; }

        /// <summary>
        /// Общая выручка от товара
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Количество заказов с этим товаром
        /// </summary>
        public int OrdersCount { get; set; }
    }

    /// <summary>
    /// DTO продаж по категориям
    /// </summary>
    public class CategorySalesDTO
    {
        /// <summary>
        /// Идентификатор категории
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        public required string CategoryName { get; set; }

        /// <summary>
        /// Количество товаров в категории
        /// </summary>
        public int ProductsCount { get; set; }

        /// <summary>
        /// Общее количество проданных товаров
        /// </summary>
        public int TotalQuantitySold { get; set; }

        /// <summary>
        /// Общая выручка по категории
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Количество заказов
        /// </summary>
        public int OrdersCount { get; set; }

        /// <summary>
        /// Средняя цена товара в категории
        /// </summary>
        public decimal AveragePrice { get; set; }
    }

    /// <summary>
    /// DTO статистики по способам оплаты
    /// </summary>
    public class PaymentMethodStatsDTO
    {
        /// <summary>
        /// Способ оплаты
        /// </summary>
        public required string PaymentMethod { get; set; }

        /// <summary>
        /// Количество заказов
        /// </summary>
        public int OrdersCount { get; set; }

        /// <summary>
        /// Общая выручка
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Процент от общей выручки
        /// </summary>
        public decimal Percentage { get; set; }
    }
}
