using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cl_backend.Models.Sales
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; } // ID пользователя, который создал заказ

        [Required]
        [StringLength(100)]
        public required string CustomerName { get; set; }

        [Required]
        [StringLength(20)]
        public required string CustomerPhone { get; set; }

        [Required]
        [StringLength(500)]
        public required string DeliveryAddress { get; set; }

        [Required]
        [StringLength(50)]
        public required string PaymentMethod { get; set; } // "Card" или "Cash"

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } // Общая стоимость заказа

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Cancelled

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}