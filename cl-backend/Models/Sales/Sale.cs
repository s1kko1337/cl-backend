using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cl_backend.Models.Sales
{
    /// <summary>
    /// Модель заказа покупателя
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Уникальный идентификатор заказа
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя создавшего заказ
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Имя покупателя
        /// </summary>
        [Required]
        [StringLength(100)]
        public required string CustomerName { get; set; }

        /// <summary>
        /// Номер телефона покупателя
        /// </summary>
        [Required]
        [StringLength(20)]
        public required string CustomerPhone { get; set; }

        /// <summary>
        /// Адрес доставки заказа
        /// </summary>
        [Required]
        [StringLength(500)]
        public required string DeliveryAddress { get; set; }

        /// <summary>
        /// Способ оплаты заказа (Card или Cash)
        /// </summary>
        [Required]
        [StringLength(50)]
        public required string PaymentMethod { get; set; }

        /// <summary>
        /// Общая стоимость заказа
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Статус заказа (Pending, Processing, Completed, Cancelled)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Дата и время создания заказа
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата и время последнего обновления заказа
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Коллекция элементов заказа (позиций)
        /// </summary>
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
