using APPDATA.Models;
using APPVIEW.Services;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace APPVIEW.Controllers
{

    public class CategoryController : Controller
    {
        private Getapi<Category> getapi;
        public INotyfService _notyf;
        public CategoryController(INotyfService notyf)
        {
            _notyf = notyf;
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
                var item = getapi.CreateObj(obj, "Category").Result;
                if (item != null)
                {
                    _notyf.Success("Thêm thành công!");
                    return RedirectToAction("GetList");
                }
                else
                {
                    _notyf.Warning("Không được để trống!");
                    return View();
                }
            }
            catch
            {
                _notyf.Error("Lỗi!");
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
                var item = getapi.UpdateObj(obj, "Category").Result;
                if (item != null)
                {
                    _notyf.Success("Edit thành công!");
                    return RedirectToAction("GetList");
                }
                else
                {
                    _notyf.Warning("Không được để trống!");
                    return View();
                }
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