using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private Getapi<Role> getapi;
        public RoleController()
        {
            getapi = new Getapi<Role>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Role");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Role obj)
        {
            try
            {
                getapi.CreateObj(obj, "Role");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]


        public async Task<IActionResult> Edit(Guid id)
        {

            var lst = getapi.GetApi("Role");
            return View(lst.Find(c => c.id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Role obj)
        {
            try
            {
                getapi.UpdateObj(obj, "Role");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "Role");
            return RedirectToAction("GetList");

        }
    }
}
