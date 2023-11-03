using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    public class CategoryController : Controller
    {
        private Getapi<Category> getapi;
        public CategoryController()
        {
            getapi = new Getapi<Category>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Category");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Category obj)
        {
            try
            {
                obj.Create_date = DateTime.Now;
                obj.Update_date = DateTime.Now;
                getapi.CreateObj(obj, "Category");
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

            var lst = getapi.GetApi("Category");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Category obj)
        {
            try
            {
                getapi.UpdateObj(obj, "Category");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "Category");
            return RedirectToAction("GetList");

        }
    }
}
