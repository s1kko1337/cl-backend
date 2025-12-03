using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    /// <summary>
    /// DTO для запроса регистрации нового пользователя
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Email адрес пользователя
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters")]
        public required string Email { get; set; }

        /// <summary>
        /// Имя пользователя для входа
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public required string Username { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
