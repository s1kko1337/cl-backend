using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    public class OrderUpdateDTO
    {
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Pending|Processing|Completed|Cancelled)$",
            ErrorMessage = "Status must be: Pending, Processing, Completed, or Cancelled")]
        public required string Status { get; set; }
    }
}