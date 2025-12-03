using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cl_backend.Models.User
{
    /// <summary>
    /// Модель пользователя системы
    /// </summary>
    public class User
    {
        /// <summary>
        /// Уникальный идентификатор пользователя
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Логин пользователя для входа в систему
        /// </summary>
        public required string Login { get; set; }

        /// <summary>
        /// Отображаемое имя пользователя
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// Хешированный пароль пользователя
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// Роль пользователя в системе (admin, user)
        /// </summary>
        public required string Role { get; set; }
    }
}
