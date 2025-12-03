using System.ComponentModel.DataAnnotations;

namespace cl_backend.DTO
{
    /// <summary>
    /// DTO для запроса входа в систему
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Email пользователя (используется как имя пользователя)
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters")]
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
