using Azure.Core;
using ImelAPI.Data;
using ImelAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("Getusers")]
        public async Task<IActionResult> GetUsers()
        {
            List<User> users = await _context.Users
                                    .Where(u => u.isDeleted == 0)
                                    .ToListAsync();

            return Ok(users);
        }

        [Authorize]
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] ImelMVC.Models.User user)
        {
            if(user == null)
            {
                return BadRequest("User data is null");
            }

            var existingUser = _context.Users.Any(u => u.Email == user.Email);

            if (existingUser)
            {
                ModelState.AddModelError("Email", "Email već postoji.");
            }

            User newUser = new()
            {
                Name = user.Name,
                Surname = user.Surname,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Email = user.Email,
                Role = user.Role,
                Status = user.Status,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            var response = await _context.SaveChangesAsync();

            if (response > 0)
            {
                return Ok();
            }

            return NotFound();
        }

        
        [Authorize]
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            return Ok(user);
        }

        [Authorize]
        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] ImelMVC.DTOs.UserDto data)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == data.Id);

            if (user != null)
            {
                user.Name = data.Name;
                user.Surname = data.Surname;
                user.Email = data.Email;
                user.Role = data.Role;
                user.Status = data.Status;

                _context.Users.Update(user);

                await _context.SaveChangesAsync();
            }


            return Ok(user);
        }

        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                user.isDeleted = 1;

                _context.Users.Update(user);

                await _context.SaveChangesAsync();

                return Ok("User deleted successfully");
            }
            else
            {
                return NotFound("User not found");
            }
        }

    }
}
