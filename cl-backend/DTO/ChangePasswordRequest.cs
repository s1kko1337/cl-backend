using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    /// <summary>
    /// DTO для запроса смены пароля пользователя
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// Текущий пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        public required string CurrentPassword { get; set; }

        /// <summary>
        /// Новый пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be between 6 and 100 characters")]
        [DataType(DataType.Password)]
        public required string NewPassword { get; set; }

        /// <summary>
        /// Подтверждение нового пароля
        /// </summary>
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }
    }
}
