using Microsoft.AspNetCore.Mvc;

namespace QLCongViecAPI.Controllers
{
    
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
