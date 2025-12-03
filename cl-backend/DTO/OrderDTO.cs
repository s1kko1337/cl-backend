namespace cl_backend.DTO
{
    /// <summary>
    /// DTO заказа с полной информацией
    /// </summary>
    public class OrderDTO
    {
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя, оформившего заказ
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Имя покупателя
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Телефон покупателя
        /// </summary>
        public string CustomerPhone { get; set; }

        /// <summary>
        /// Адрес доставки
        /// </summary>
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// Способ оплаты
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Общая сумма заказа
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Статус заказа (Pending, Processing, Completed, Cancelled)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Дата создания заказа
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата последнего обновления заказа
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Список товаров в заказе
        /// </summary>
        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    }

    /// <summary>
    /// DTO элемента заказа
    /// </summary>
    public class OrderItemDTO
    {
        /// <summary>
        /// Идентификатор элемента заказа
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор товара
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Цена товара на момент покупки
        /// </summary>
        public decimal PriceAtPurchase { get; set; }

        /// <summary>
        /// Количество товара
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Промежуточная сумма (цена × количество)
        /// </summary>
        public decimal Subtotal { get; set; }
    }
}
