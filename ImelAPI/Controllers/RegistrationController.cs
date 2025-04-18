using ImelAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using ImelAPI.Data;
using ImelMVC.DTOs;

namespace ImelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public RegistrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegistrationDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email and password are required.");
            }

            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return Conflict("A user with this email already exists.");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            User user = new User
            {
                Name = request.Name,
                Surname = request.Surname,
                HashedPassword = hashedPassword,
                Email = request.Email,
                Role = request.Role,
                Status = request.Status,
                CreatedAt = DateTime.Now,
                VersionNum = request.VersionNum,
                isDeleted = request.isDeleted
            };

            _context.Users.Add(user);
            _context.SaveChanges();


            return Ok(new { Message = "User registered successfully." , Password = hashedPassword });
        }
    }
}
