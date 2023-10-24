using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    public class PaymentMethodDetailsController : Controller
    {
        private Getapi<PaymentMethodDetail> getapi;
        public PaymentMethodDetailsController()
        {
            getapi = new Getapi<PaymentMethodDetail>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("PaymentMethodDetail");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(PaymentMethodDetail obj)
        {
            try
            {
                getapi.CreateObj(obj, "PaymentMethodDetail");
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

            var lst = getapi.GetApi("PaymentMethodDetail");
            return View(lst.Find(c => c.id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(PaymentMethodDetail obj)
        {
            try
            {
                getapi.UpdateObj(obj, "PaymentMethodDetail");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "PaymentMethodDetail");
            return RedirectToAction("GetList");

        }
    }
}
