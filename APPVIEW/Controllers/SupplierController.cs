using APPDATA.Models;
using APPVIEW.Services;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SupplierController : Controller
    {
        private Getapi<Supplier> getapi;
        public INotyfService _notyf;
        public SupplierController(INotyfService notyf)
        {
            _notyf = notyf;
            getapi = new Getapi<Supplier>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Supplier");
            return View(obj);
        }
        public async Task<IActionResult> Search(string searchTerm)
        {
            var lstSupplier = getapi.GetApi("Supplier").ToList();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View("GetList", lstSupplier);
            }
            var searchResult = lstSupplier
                .Where(v =>
                    v.Suppliercode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Address.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.PhoneNumber.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            if (searchResult.Any())
            {
                return View("GetList", searchResult);
            }

            _notyf.Information("Voucher không tồn tại");
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Supplier obj)
        {
            try
            {
                var item = getapi.CreateObj(obj, "Supplier").Result;
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
                return View();
            }
        }
        [HttpGet]


        public async Task<IActionResult> Edit(Guid id)
        {

            var lst = getapi.GetApi("Supplier");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Supplier obj)
        {
            try
            {
                var item = getapi.UpdateObj(obj, "Supplier").Result;
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
                _notyf.Error("Lỗi!");
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            await getapi.DeleteObj(id, "Supplier");
            return RedirectToAction("GetList");

        }

    }
}