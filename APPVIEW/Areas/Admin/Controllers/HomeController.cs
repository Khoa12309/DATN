using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
