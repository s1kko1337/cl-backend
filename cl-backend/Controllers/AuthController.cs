using cl_backend.DbContexts;
using cl_backend.DTO;
using cl_backend.Extensions;
using cl_backend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace cl_backend.Controllers
{
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ApplicationContext _context;

        public AuthController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost("/register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user already exists
            if (_context.Users.Any(u => u.Login == request.Username))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Username already exists!"
                });
            }

            try
            {
                var user = request.ToEntity();
                user.Password = AuthUtils.HashPassword(user.Password);

                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok(new AuthResponse
                {
                    Success = true,
                    Message = "User registered successfully!",
                    User = user.ToDTO()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Message = $"An error occurred during registration: {ex.Message}"
                });
            }
        }

        [HttpPost("/login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = GetIdentity(request.Username, request.Password);
            if (identity == null)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid username or password!"
                });
            }

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Login == request.Username);

                var now = DateTime.UtcNow;
                var jwt = new JwtSecurityToken
                (
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return Ok(new AuthResponse
                {
                    Success = true,
                    Message = "Login successful!",
                    User = user?.ToDTO(),
                    Token = encodedJwt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Message = $"An error occurred during login: {ex.Message}"
                });
            }
        }

        private ClaimsIdentity? GetIdentity(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Login == username);
            if (user == null)
            {
                return null;
            }

            if (!AuthUtils.VerifyPassword(password, user.Password))
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}