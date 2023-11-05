using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ColorController : Controller
    {
        private Getapi<Color> getapi;
        public ColorController()
        {
            getapi = new Getapi<Color>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Color");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Color obj)
        {
            try
            {
                getapi.CreateObj(obj, "Color");
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

            var lst = getapi.GetApi("Color");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Color obj)
        {
            try
            {
                getapi.UpdateObj(obj, "Color");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "Color");
            return RedirectToAction("GetList");

        }
    }
}
