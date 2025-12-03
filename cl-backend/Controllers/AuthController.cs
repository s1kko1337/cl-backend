using cl_backend.DbContexts;
using cl_backend.DTO;
using cl_backend.Extensions;
using cl_backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace cl_backend.Controllers
{
    /// <summary>
    /// Контроллер для аутентификации и управления пользователями
    /// </summary>
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ApplicationContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера аутентификации
        /// </summary>
        /// <param name="context">Контекст базы данных приложения</param>
        public AuthController(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Регистрирует нового пользователя в системе
        /// </summary>
        /// <param name="request">Данные для регистрации пользователя</param>
        /// <returns>Ответ с результатом регистрации и данными пользователя</returns>
        [HttpPost("/register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Users.Any(u => u.Login == request.Email))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Email already exists!"
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

        /// <summary>
        /// Выполняет вход пользователя в систему
        /// </summary>
        /// <param name="request">Данные для входа (username и password)</param>
        /// <returns>Ответ с JWT токеном и данными пользователя при успешном входе</returns>
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

        /// <summary>
        /// Изменяет пароль текущего аутентифицированного пользователя
        /// </summary>
        /// <param name="request">Данные для смены пароля (текущий и новый пароль)</param>
        /// <returns>Ответ с результатом операции смены пароля</returns>
        [Authorize]
        [HttpPost("/change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userLogin = User.Identity?.Name;
                if (string.IsNullOrEmpty(userLogin))
                {
                    return Unauthorized(new AuthResponse
                    {
                        Success = false,
                        Message = "User not authenticated"
                    });
                }

                var user = _context.Users.FirstOrDefault(u => u.Login == userLogin);
                if (user == null)
                {
                    return NotFound(new AuthResponse
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                if (!AuthUtils.VerifyPassword(request.CurrentPassword, user.Password))
                {
                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        Message = "Current password is incorrect"
                    });
                }

                user.Password = AuthUtils.HashPassword(request.NewPassword);
                _context.SaveChanges();

                return Ok(new AuthResponse
                {
                    Success = true,
                    Message = "Password changed successfully",
                    User = user.ToDTO()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Message = $"An error occurred while changing password: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Создает ClaimsIdentity для пользователя после проверки учетных данных
        /// </summary>
        /// <param name="username">Имя пользователя (email)</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>ClaimsIdentity пользователя или null если учетные данные неверны</returns>
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