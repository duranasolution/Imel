using ImelAPI.Data;
using ImelAPI.Models;
using ImelAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ImelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly LoginAttemptService _loginAttemptService;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration config, LoginAttemptService loginAttemptService, ApplicationDbContext context)
        {
            _config = config;
            _loginAttemptService = loginAttemptService;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (_loginAttemptService.IsAccountLocked(request.Email))
            {
                return Unauthorized("Račun je zaključan zbog previše neuspjelih pokušaja. Pokušajte ponovo nakon 15 minuta.");
            }

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {

                return BadRequest("Email i lozinka su obavezni.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user != null)
            {
                if (request.Email != user.Email || !BCrypt.Net.BCrypt.Verify(request.Password, user.HashedPassword))
                {
                    _loginAttemptService.RecordFailedAttempt(request.Email);
                    return Unauthorized("Pogrešan email ili lozinka.");
                }
            }
            else
            {
                return NotFound();
            }
                _loginAttemptService.ResetFailedAttempts(request.Email);

            var token = GenerateJwtToken(user.Email, user.Role, user.Surname, user.Name, user.UserSpecificId);
            return Ok(new { token });
        }

        private string GenerateJwtToken(string email, string role, string surname, string name, string userSpecificId)
        {
            var claims = new[] {
                new Claim("email", email),
                new Claim("role", role),
                new Claim("surname", surname),
                new Claim("name", name),
                new Claim("userSpecificId", userSpecificId),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
