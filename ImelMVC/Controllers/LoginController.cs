using ImelMVC.Models;
using ImelMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ImelMVC.Controllers
{
    public class LoginController : Controller
    {

        private readonly HttpClient _client;
        private readonly Methods _methods;
        private readonly IConfiguration _config;

        public LoginController(HttpClient client, Methods methods, IConfiguration config)
        {
            _client = client;
            _methods = methods;
            _config = config;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginData)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(_config.GetValue<string>("Api:BaseApi") + "auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    string stringToken = _methods.MakeStringFromToken(token);

                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(stringToken);

                    var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                    var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                    var surnameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "surname")?.Value;
                    var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

                    HttpContext.Session.SetString("token", stringToken);
                    HttpContext.Session.SetString("email", emailClaim);
                    HttpContext.Session.SetString("role", roleClaim);
                    HttpContext.Session.SetString("surname", surnameClaim);
                    HttpContext.Session.SetString("name", nameClaim);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Neuspješna prijava. Provjeri email i lozinku.");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Došlo je do greške prilikom prijave: {ex.Message}");
                return View();
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
