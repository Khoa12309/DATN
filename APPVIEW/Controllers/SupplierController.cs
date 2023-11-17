using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SupplierController : Controller
    {
        private Getapi<Supplier> getapi;
        public SupplierController()
        {
            getapi = new Getapi<Supplier>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Supplier");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Supplier obj)
        {
            try
            {
              await  getapi.CreateObj(obj, "Supplier");
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

            var lst = getapi.GetApi("Supplier");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Supplier obj)
        {
            try
            {
              await  getapi.UpdateObj(obj, "Supplier");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
          await  getapi.DeleteObj(id, "Supplier");
            return RedirectToAction("GetList");

        }

    }
}
