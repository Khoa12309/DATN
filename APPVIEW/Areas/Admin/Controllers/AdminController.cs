using _APPAPI.Service;
using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin,Staff")]
    public class AdminController : Controller
    {
        private Getapi<Account> getapiAc;

        public AdminController()
        {
            getapiAc = new Getapi<Account>();
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {

                var Uid = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                var acc = getapiAc.GetApi("Account").FirstOrDefault(c => c.Id.ToString() == Uid);
                SessionService.SetObjToJson(HttpContext.Session, "Account", acc);
            }
            return View();
        }
    }
}
