using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
