using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Kontakty.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kontakty.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Nieprawidłowe dane logowania.");
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);

            if (existingUser != null)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, existingUser.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Ok("Logowanie udane.");
            }

            return Unauthorized("Nieprawidłowy email lub hasło.");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Wylogowano pomyślnie.");
        }

        [HttpGet("user")]
        public IActionResult GetUser()
        {
            var user = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (!string.IsNullOrEmpty(user))
            {
                return Ok($"Zalogowany użytkownik: {user}");
            }

            return Unauthorized("Brak zalogowanego użytkownika.");
        }
    }
}
