using Azure.Core;
using ImelAPI.Data;
using ImelAPI.Models;
using ImelAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AuditLogService _auditLogService;
        private readonly Methods _methods;
        public UserController(ApplicationDbContext context, AuditLogService auditLog, Methods methods)
        {
            _context = context;
            _auditLogService = auditLog;
            _methods = methods;
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            List<User> users = await _context.Users
                                    .Where(u => u.isDeleted == 0 && u.Status == "Active")
                                    .ToListAsync();

            return Ok(users);
        }

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
                return BadRequest(ModelState);
            }

            User newUser = new()
            {
                UserSpecificId = Guid.NewGuid().ToString(),
                Name = user.Name,
                Surname = user.Surname,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Email = user.Email,
                Role = user.Role,
                Status = user.Status,
                CreatedAt = DateTime.Now,
                VersionNum = user.VersionNum
            };

            _context.Users.Add(newUser);
            var response = await _context.SaveChangesAsync();

            string performedBy = User.Claims.FirstOrDefault(c => c.Type == "userSpecificId")?.Value;

            _auditLogService.LogAction("Dodavanje Korisnika", newUser.UserSpecificId, performedBy, "User");


            if (response > 0)
            {
                return Ok();
            }

            return NotFound();
        }

        
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            return Ok(user);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] ImelMVC.DTOs.UserDto data)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == data.Id);

            if (user != null)
            {

                if (user.Status != data.Status &&
                    user.Name == data.Name &&
                    user.Surname == data.Surname &&
                    user.Email == data.Email &&
                    user.Role == data.Role)
                {

                    user.Status = data.Status;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();


                    string performedBy = User.Claims.FirstOrDefault(c => c.Type == "userSpecificId")?.Value;

                    _auditLogService.LogAction("Promjena statusa", user.UserSpecificId, performedBy, "User");

                }
                else
                {
                    user.Status = "Inactive";

                    _context.Users.Update(user);

                    User newUpdatedUser = new()
                    {
                        UserSpecificId = data.UserSpecificId,
                        Name = data.Name,
                        Surname = data.Surname,
                        HashedPassword = user.HashedPassword,
                        Email = data.Email,
                        Role = data.Role,
                        Status = data.Status,
                        CreatedAt = user.CreatedAt,
                        VersionNum = user.VersionNum + 1,
                    };

                    _context.Users.Add(newUpdatedUser);

                    await _context.SaveChangesAsync();

                    string performedBy = User.Claims.FirstOrDefault(c => c.Type == "userSpecificId")?.Value;

                    _auditLogService.LogAction("Promjena podataka", newUpdatedUser.UserSpecificId, performedBy, "User");


                }


            }


            return Ok(user);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                user.isDeleted = 1;

                _context.Users.Update(user);

                await _context.SaveChangesAsync();

                string performedBy = User.Claims.FirstOrDefault(c => c.Type == "userSpecificId")?.Value;

                _auditLogService.LogAction("Brisanke podataka", user.UserSpecificId, performedBy, "User");


                return Ok("User deleted successfully");
            }
            else
            {
                return NotFound("User not found");
            }
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var latestUsers = await _context.Users
                .GroupBy(u => u.Email) 
                .Select(g => g.OrderByDescending(u => u.VersionNum).First())
                .ToListAsync();

            return Ok(latestUsers);
        }

        [HttpGet("ShowAllVersions")]
        public async Task<IActionResult> ShowAllVersions()
        {
            var allUsers = await _context.Users
                .OrderBy(u => u.Email)
                .ThenByDescending(u => u.VersionNum)
                .ToListAsync();

            return Ok(allUsers);
        }


        [HttpGet("SearchUser/{searchInput}/{role}")]
        public async Task<IActionResult> SearchUser(string searchInput, string role)
        {
            if (string.IsNullOrWhiteSpace(searchInput) && string.IsNullOrWhiteSpace(role))
            {
                return BadRequest("Search input and role cannot both be empty.");
            }


            if (!string.IsNullOrEmpty(searchInput))
            {

                IQueryable<User> query = _context.Users
                                        .Where(u => u.isDeleted == 0 &&
                                            (u.Name.Contains(searchInput) ||
                                             u.Surname.Contains(searchInput) ||
                                             u.Email.Contains(searchInput)));
                if (!string.IsNullOrEmpty(role))
                {
                    if (role == "active")
                    {
                        query = query.Where(u => u.Status == "Active");
                    }
                    else if (role == "inactive")
                    {
                        query = query.Where(u => u.Status == "Inactive");
                    }
                    else
                    {
                        query = query.Where(u =>(u.Name.Contains(searchInput) ||
                                                 u.Surname.Contains(searchInput) ||
                                                 u.Email.Contains(searchInput)));
                    }
                }

                var users = await query.ToListAsync();
                return Ok(users);
            }



            return NotFound();
        }

        [HttpGet("History/{id}")]
        public async Task<IActionResult> History(string id)
        {
            var historyData = await _context.AuditLog.Where(a => a.UserId == id).ToListAsync();

            return Ok(historyData);
        }


        [HttpGet("Download/{type}")]
        public async Task<IActionResult> Download(string type)
        {
            if(type == "CSV")
            {
                return await _methods.Download("csv");
            }
            else if (type == "xlsx")
            {
                return await _methods.Download("xlsx");
            }
            else if(type == "PDF")
            {
                return await _methods.Download("pdf");
            }
            else
            {
                return BadRequest("Invalid file type");
            }
        }


    }
}
