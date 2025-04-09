using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ImelMVC.Models;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using ImelMVC.Services;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;

namespace ImelMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;


    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;

    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
