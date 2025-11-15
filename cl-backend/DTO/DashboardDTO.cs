namespace cl_backend.DTO
{
    // Dashboard с общей статистикой
    public class DashboardSummaryDTO
    {
        // Общая статистика
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }

        // Статистика за сегодня
        public int TodayOrders { get; set; }
        public decimal TodayRevenue { get; set; }

        // Статистика за неделю
        public int WeekOrders { get; set; }
        public decimal WeekRevenue { get; set; }

        // Статистика за месяц
        public int MonthOrders { get; set; }
        public decimal MonthRevenue { get; set; }

        // KPI
        public decimal AverageOrderValue { get; set; }
        public int LowStockProductsCount { get; set; }
        public int OutOfStockProductsCount { get; set; }
        public int PendingOrdersCount { get; set; }

        // Статус заказов
        public Dictionary<string, int> OrdersByStatus { get; set; } = new Dictionary<string, int>();

        // Последние заказы
        public List<RecentOrderDTO> RecentOrders { get; set; } = new List<RecentOrderDTO>();
    }

    // Недавние заказы для Dashboard
    public class RecentOrderDTO
    {
        public int Id { get; set; }
        public required string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Алерты для Dashboard
    public class DashboardAlertDTO
    {
        public required string Type { get; set; } // "warning", "info", "error"
        public required string Message { get; set; }
        public int Count { get; set; }
    }
}
