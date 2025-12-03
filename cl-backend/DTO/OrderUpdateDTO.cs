using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    /// <summary>
    /// DTO для обновления статуса заказа
    /// </summary>
    public class OrderUpdateDTO
    {
        /// <summary>
        /// Новый статус заказа (Pending, Processing, Completed, Cancelled)
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Pending|Processing|Completed|Cancelled)$",
            ErrorMessage = "Status must be: Pending, Processing, Completed, or Cancelled")]
        public required string Status { get; set; }
    }
}
