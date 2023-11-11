using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class VoucherController : Controller
    {
        private Getapi<Voucher> getapi;
        public VoucherController()
        {
            getapi = new Getapi<Voucher>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Voucher");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: VoucherController/Create
        [HttpPost]
        public async Task<IActionResult> Create(Voucher obj)
        {
            try
            {
                getapi.CreateObj(obj, "Voucher");
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

            var lst = getapi.GetApi("Voucher");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Voucher obj)
        {
            try
            {
                getapi.UpdateObj(obj, "Voucher");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            
            getapi.DeleteObj(id, "Voucher");
           return RedirectToAction("GetList");

        }
    }
}
