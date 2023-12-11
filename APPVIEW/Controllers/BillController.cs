using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    public class BillController : Controller
    {
        private Getapi<Bill> getapi;
        public BillController()
        {
            getapi = new Getapi<Bill>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Bill");
            return View(obj);
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            var lstBill = getapi.GetApi("Bill");

            var searchResult = lstBill
                .Where(v =>
                    v.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Status.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            if (searchResult.Any())
            {
                return View("GetList", searchResult);
            }

            return NotFound("Voucher không tồn tại");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Bill obj)
        {
            try
            {
                getapi.CreateObj(obj, "Bill");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }



        public async Task<IActionResult> Edit(Guid id)
        {

            var lst = getapi.GetApi("Bill");
            return View(lst.Find(c => c.id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Bill obj)
        {
            try
            {
                getapi.UpdateObj(obj, "Bill");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "Bill");
            return RedirectToAction("GetList");

        }
    }
}
