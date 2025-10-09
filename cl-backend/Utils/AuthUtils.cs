using System.Security.Cryptography;
using System.Text;


namespace cl_backend.Utils
{
    public class AuthUtils
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

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
