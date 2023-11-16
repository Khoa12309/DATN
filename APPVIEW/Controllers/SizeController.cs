using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    public class SizeController : Controller
    {
        private Getapi<Size> getapi;
        public SizeController()
        {
            getapi = new Getapi<Size>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Size");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Size obj)
        {
            try
            {
                obj.Id=Guid.NewGuid();  
              await  getapi.CreateObj(obj, "Size");
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

            var lst = getapi.GetApi("Size");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Size obj)
        {
            try
            {
              await  getapi.UpdateObj(obj, "Size");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
          await  getapi.DeleteObj(id, "Size");
            return RedirectToAction("GetList");

        }
    }
}
