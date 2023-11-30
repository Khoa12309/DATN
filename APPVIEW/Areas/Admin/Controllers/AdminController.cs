using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin,Staff")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
