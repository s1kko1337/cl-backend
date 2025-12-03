namespace cl_backend.DTO
{
    /// <summary>
    /// DTO ответа на запросы аутентификации
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// Указывает, была ли операция успешной
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Сообщение о результате операции
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Данные пользователя (если операция успешна)
        /// </summary>
        public UserDTO? User { get; set; }

        /// <summary>
        /// JWT токен для авторизации (если операция успешна)
        /// </summary>
        public string? Token { get; set; }
    }
}
