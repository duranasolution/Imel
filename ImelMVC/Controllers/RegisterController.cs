using ImelMVC.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace ImelMVC.Controllers
{
    public class RegisterController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config; 

        public RegisterController(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationDto registerData)
        {
            if (ModelState.IsValid && registerData.Password == registerData.ConfirmPassword)
            {
                var jsonContent = JsonConvert.SerializeObject(registerData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = _httpClient.PostAsJsonAsync(_config.GetValue<string>("Api:BaseApi") + "Registration/Register", registerData).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Login");
                }
                else if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    TempData["Message"] = "Korisnik sa tom email adresom već postoji.";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Došlo je do greške prilikom registracije.");
                }
            }

            return RedirectToAction("Index", "Login");
        }
    }
}
