using ImelMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ImelMVC.Controllers
{
    public class UserController : Controller
    {

        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public UserController(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            string token = HttpContext.Session.GetString("token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Login");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync(_config.GetValue<string>("Api:BaseApi") + "user/GetUsers");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<User>>(json);

                return View(users);
            }

            return Unauthorized();
        }
    }
}
