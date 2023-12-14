using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class BillDetailController : Controller
    {
        private Getapi<BillDetail> getapi;
        public BillDetailController()
        {
            getapi = new Getapi<BillDetail>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("BillDetail");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(BillDetail obj)
        {
            try
            {
                getapi.CreateObj(obj, "BillDetail");
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

            var lst = getapi.GetApi("BillDetail");
            return View(lst.Find(c => c.id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(BillDetail obj)
        {
            try
            {
                getapi.UpdateObj(obj, "BillDetail");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "BillDetail");
            return RedirectToAction("GetList");

        }
    }
}
