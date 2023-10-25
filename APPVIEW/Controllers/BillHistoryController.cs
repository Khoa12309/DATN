using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    public class BillHistoryController : Controller
    {
        private Getapi<BillHistory> getapi;
        public BillHistoryController()
        {
            getapi = new Getapi<BillHistory>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("BillHistory");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: VoucherController/Create
        [HttpPost]
        public async Task<IActionResult> Create(BillHistory obj)
        {
            try
            {
                getapi.CreateObj(obj, "BillHistory");
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

            var lst = getapi.GetApi("BillHistory");
            return View(lst.Find(c => c.id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(BillHistory obj)
        {
            try
            {
                getapi.UpdateObj(obj, "BillHistory");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "BillHistory");
            return RedirectToAction("GetList");

        }
    }
}
