using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace cl_backend
{
    public class AuthOptions
    {
        public const string ISSUER = "cl_backend_App_Server";
        public const string AUDIENCE = "cl_backend_App_Client";
        public const string KEY = "FE587B70-3E42-4813-8604-E340134B490A";
        public const int LIFETIME = 1;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
