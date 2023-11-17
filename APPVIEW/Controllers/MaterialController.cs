using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    public class MaterialController : Controller
    {
        private Getapi<Material> getapi;
        public MaterialController()
        {
            getapi = new Getapi<Material>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Material");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Material obj)
        {
            try
            {
                getapi.CreateObj(obj, "Material");
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

            var lst = getapi.GetApi("Material");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Material obj)
        {
            try
            {
                getapi.UpdateObj(obj, "Material");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "Material");
            return RedirectToAction("GetList");

        }
    }
}
