using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace cl_backend
{
    /// <summary>
    /// Параметры конфигурации JWT аутентификации
    /// </summary>
    public class AuthOptions
    {
        /// <summary>
        /// Издатель токена
        /// </summary>
        public const string ISSUER = "cl_backend_App_Server";

        /// <summary>
        /// Получатель токена
        /// </summary>
        public const string AUDIENCE = "cl_backend_App_Client";

        /// <summary>
        /// Секретный ключ для подписи токенов
        /// </summary>
        public const string KEY = "FE587B70-3E42-4813-8604-E340134B490A";

        /// <summary>
        /// Время жизни токена в минутах
        /// </summary>
        public const int LIFETIME = 800;

        /// <summary>
        /// Получает симметричный ключ безопасности для подписи и валидации JWT токенов
        /// </summary>
        /// <returns>Симметричный ключ безопасности</returns>
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
