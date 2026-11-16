using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TodoMvc.Models;

namespace TodoMvc.Controllers;

public class HomeController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}