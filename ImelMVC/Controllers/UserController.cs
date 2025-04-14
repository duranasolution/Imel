using ImelMVC.Models;
using ImelMVC.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ImelMVC.Controllers
{
    public class UserController : Controller
    {

        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public UserController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _client = httpClientFactory.CreateClient("ApiClient");
            _config = config;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _client.GetAsync(_config.GetValue<string>("Api:BaseApi") + "user/GetUsers");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<User>>(json);

                return View(users);
            }

            return Unauthorized();
        }


        [HttpGet]
        public  IActionResult Add()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Add(User user)
        {
            if (ModelState.IsValid)
            {
                var userJson = JsonConvert.SerializeObject(user);
                var content = new StringContent(userJson, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(_config.GetValue<string>("Api:BaseApi") + "user/add", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }

            return View("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var rseponse = await _client.GetAsync(_config.GetValue<string>("Api:BaseApi") + $"user/Details/{id}");

            if(rseponse.IsSuccessStatusCode)
            {
                var json = await rseponse.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(json);
                return View(user);
            }   


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserDto user)
        {
            if (ModelState.IsValid)
            {

                var userJson = JsonConvert.SerializeObject(user);
                var content = new StringContent(userJson, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(_config.GetValue<string>("Api:BaseApi") + "user/Update", content);

            }


            return RedirectToAction("Index", "User");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if(id != 0)
            {
                var response = await _client.DeleteAsync(_config.GetValue<string>("Api:BaseApi") + $"user/Delete/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "User");
                }
            }
            return View();
        }
    }
}
