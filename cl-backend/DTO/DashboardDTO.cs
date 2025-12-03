namespace cl_backend.DTO
{
    /// <summary>
    /// DTO сводной статистики для дашборда администратора
    /// </summary>
    public class DashboardSummaryDTO
    {
        /// <summary>
        /// Общее количество заказов
        /// </summary>
        public int TotalOrders { get; set; }

        /// <summary>
        /// Общее количество товаров
        /// </summary>
        public int TotalProducts { get; set; }

        /// <summary>
        /// Общее количество категорий
        /// </summary>
        public int TotalCategories { get; set; }

        /// <summary>
        /// Общее количество пользователей
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// Общая выручка
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Количество заказов за сегодня
        /// </summary>
        public int TodayOrders { get; set; }

        /// <summary>
        /// Выручка за сегодня
        /// </summary>
        public decimal TodayRevenue { get; set; }

        /// <summary>
        /// Количество заказов за неделю
        /// </summary>
        public int WeekOrders { get; set; }

        /// <summary>
        /// Выручка за неделю
        /// </summary>
        public decimal WeekRevenue { get; set; }

        /// <summary>
        /// Количество заказов за месяц
        /// </summary>
        public int MonthOrders { get; set; }

        /// <summary>
        /// Выручка за месяц
        /// </summary>
        public decimal MonthRevenue { get; set; }

        /// <summary>
        /// Средняя стоимость заказа
        /// </summary>
        public decimal AverageOrderValue { get; set; }

        /// <summary>
        /// Количество товаров с низким остатком
        /// </summary>
        public int LowStockProductsCount { get; set; }

        /// <summary>
        /// Количество товаров которые закончились на складе
        /// </summary>
        public int OutOfStockProductsCount { get; set; }

        /// <summary>
        /// Количество необработанных заказов
        /// </summary>
        public int PendingOrdersCount { get; set; }

        /// <summary>
        /// Распределение заказов по статусам
        /// </summary>
        public Dictionary<string, int> OrdersByStatus { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Последние заказы
        /// </summary>
        public List<RecentOrderDTO> RecentOrders { get; set; } = new List<RecentOrderDTO>();
    }

    /// <summary>
    /// DTO недавнего заказа для дашборда
    /// </summary>
    public class RecentOrderDTO
    {
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Имя покупателя
        /// </summary>
        public required string CustomerName { get; set; }

        /// <summary>
        /// Общая стоимость заказа
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Статус заказа
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// Дата создания заказа
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO алерта для дашборда
    /// </summary>
    public class DashboardAlertDTO
    {
        /// <summary>
        /// Тип алерта (warning, info, error)
        /// </summary>
        public required string Type { get; set; }

        /// <summary>
        /// Сообщение алерта
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// Количество элементов требующих внимания
        /// </summary>
        public int Count { get; set; }
    }
}
