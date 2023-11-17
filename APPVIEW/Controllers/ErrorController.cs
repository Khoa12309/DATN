using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
