using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cl_backend.Models.Sales
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }

        [Required]
        [StringLength(200)]
        public required string ProductName { get; set; } // Название на момент покупки

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtPurchase { get; set; } // Цена на момент покупки

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; } // PriceAtPurchase * Quantity
    }
}