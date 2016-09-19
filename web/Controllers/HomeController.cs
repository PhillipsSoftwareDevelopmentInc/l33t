using Microsoft.AspNetCore.Mvc;

namespace Receiptze
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index() => View();
    }
}