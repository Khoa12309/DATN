using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    public class ImageController : Controller
    {
        private Getapi<Image> getapi;
        public ImageController()
        {
            getapi = new Getapi<Image>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Image");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Image obj)
        {
            try
            {
                getapi.CreateObj(obj, "Image");
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

            var lst = getapi.GetApi("Image");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Image obj)
        {
            try
            {
                getapi.UpdateObj(obj, "Image");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "Image");
            return RedirectToAction("GetList");

        }
    }
}
