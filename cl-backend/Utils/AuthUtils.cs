using System.Security.Cryptography;
using System.Text;


namespace cl_backend.Utils
{
    /// <summary>
    /// Утилиты для работы с аутентификацией и хешированием паролей
    /// </summary>
    public class AuthUtils
    {
        /// <summary>
        /// Хеширует пароль используя алгоритм SHA256
        /// </summary>
        /// <param name="password">Пароль в открытом виде</param>
        /// <returns>Хешированный пароль в формате Base64</returns>
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// Проверяет соответствие пароля хешированному значению
        /// </summary>
        /// <param name="password">Пароль в открытом виде для проверки</param>
        /// <param name="hashedpassword">Хешированный пароль в формате Base64</param>
        /// <returns>True если пароли совпадают, иначе False</returns>
        public static bool VerifyPassword(string password, string hashedpassword)
        {
            var hashedPasswordBytes = Convert.FromBase64String(hashedpassword);
            using (var sha256 = SHA256.Create())
            {
                var computedHashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return hashedPasswordBytes.SequenceEqual(computedHashBytes);
            }
        }
    }
}
