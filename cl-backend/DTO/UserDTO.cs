namespace cl_backend.DTO
{
    /// <summary>
    /// DTO пользователя для передачи клиенту
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public required string Login { get; set; }

        /// <summary>
        /// Имя пользователя для отображения
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// Роль пользователя в системе
        /// </summary>
        public required string Role { get; set; }
    }
}
