using cl_backend.DbContexts;
using cl_backend.DTO;
using cl_backend.Models.User;
using cl_backend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace cl_backend.Controllers
{
    //[Route("/api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private ApplicationContext _context;

        public AuthController()
        {
            _context = new ApplicationContext();
        }

        [HttpPost("/register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            _context.Users.Add(new Models.User.User { Login = request.Username, Password = AuthUtils.HashPassword(request.Password), Role = "user" });
            var id = _context.SaveChanges();
            return Ok(id);
        }

        [HttpPost("/login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var identity = GetIdentity(request.Username, request.Password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password!" });
            }
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken
                (
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
            
            return Ok(JsonConvert.SerializeObject(response));  
        }

        private ClaimsIdentity GetIdentity(string username, string password) 
        {
            var user = _context.Users.FirstOrDefault(u => u.Login == username);
            if (user == null)
            {
                return null;
            }
            if (!AuthUtils.VerifyPassword(password, user.Password)) { return null; }

            var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
