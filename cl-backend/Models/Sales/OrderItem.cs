using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cl_backend.Models.Sales
{
    /// <summary>
    /// Модель элемента заказа (позиция в заказе)
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// Уникальный идентификатор элемента заказа
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор заказа к которому принадлежит элемент
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Навигационное свойство заказа
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// Идентификатор товара
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Название товара на момент покупки
        /// </summary>
        [Required]
        [StringLength(200)]
        public required string ProductName { get; set; }

        /// <summary>
        /// Цена товара на момент покупки
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtPurchase { get; set; }

        /// <summary>
        /// Количество единиц товара в заказе
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Сумма по позиции (цена умноженная на количество)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
    }
}
