using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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
              await  getapi.CreateObj(obj, "Category");
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
              await  getapi.UpdateObj(obj, "Category");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {

            if (await getapi.DeleteObj(id, "Category"))
            {
                var lst = getapi.GetApi("Category").Find(c => c.Id == id);
                lst.Status = 0;
                await getapi.UpdateObj(lst, "Category");
            }
            return RedirectToAction("GetList");

        }
    }
}
