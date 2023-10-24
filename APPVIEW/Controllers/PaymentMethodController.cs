using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    public class PaymentMethodController : Controller
    {
        private Getapi<PaymentMethod> getapi;
        public PaymentMethodController()
        {
            getapi = new Getapi<PaymentMethod>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("PaymentMethod");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(PaymentMethod obj)
        {
            try
            {
                getapi.CreateObj(obj, "PaymentMethod");
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

            var lst = getapi.GetApi("PaymentMethod");
            return View(lst.Find(c => c.id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(PaymentMethod obj)
        {
            try
            {
                getapi.UpdateObj(obj, "PaymentMethod");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "PaymentMethod");
            return RedirectToAction("GetList");

        }
    }
}
