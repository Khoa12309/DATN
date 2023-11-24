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

        public async Task<IActionResult> Search(string searchTerm)
        {
            var lstRole = getapi.GetApi("Role").ToList();

            var searchResult = lstRole
                .Where(v =>
                    v.name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Status.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            if (searchResult.Any())
            {
                return View("GetList", searchResult);
            }

            return NotFound("Voucher không tồn tại");
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
             await   getapi.CreateObj(obj, "Role");
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
              await  getapi.UpdateObj(obj, "Role");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
          await  getapi.DeleteObj(id, "Role");
            return RedirectToAction("GetList");

        }
    }
}
