using Microsoft.AspNetCore.Mvc;

namespace NycTaxiSearch.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
