using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    public class OrderCreateDTO
    {
        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Customer name must be between 2 and 100 characters")]
        public required string CustomerName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Phone number must be between 5 and 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public required string CustomerPhone { get; set; }

        [Required(ErrorMessage = "Delivery address is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Delivery address must be between 10 and 500 characters")]
        public required string DeliveryAddress { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [RegularExpression("^(Card|Cash)$", ErrorMessage = "Payment method must be either 'Card' or 'Cash'")]
        public required string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Order items are required")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public required List<OrderItemCreateDTO> OrderItems { get; set; }
    }

    public class OrderItemCreateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}