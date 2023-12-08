using _APPAPI.ViewModels;
using APPDATA.Models;
using APPVIEW.Services;
using APPVIEW.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Policy;

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

        public async Task<IActionResult> Search(string searchTerm)
        {
            var lstSize = getapi.GetApi("Size").ToList();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View("GetList", lstSize);
            }
            var searchResult = lstSize
                .Where(v =>
                    v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Status.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            if (searchResult.Any())
            {
                return View("GetList", searchResult);
            }

            return NotFound("Size không tồn tại");
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
