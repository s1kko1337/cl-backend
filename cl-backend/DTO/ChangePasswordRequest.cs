using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        public required string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be between 6 and 100 characters")]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }
    }
}
