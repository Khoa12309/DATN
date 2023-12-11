using APPDATA.Models;
using APPVIEW.Services;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VoucherController : Controller
    {
        private Getapi<Voucher> getapi;
        private Getapi<Category> getapiCategory;
        public INotyfService _notyf;
        public VoucherController(INotyfService notyf)
        {
            _notyf = notyf;
            getapi = new Getapi<Voucher>();
            getapiCategory = new Getapi<Category>();
        }

        public async Task<IActionResult> GetList()
        {
            ViewBag.Category = await getapiCategory.GetApia("Category");
            var obj = getapi.GetApi("Voucher");
            return View(obj);
        }
        public async Task<IActionResult> Search(string searchTerm)
        {
            var lstVoucher = getapi.GetApi("Voucher").ToList();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View("GetList", lstVoucher);
            }
            var searchResult = lstVoucher
                .Where(v =>
                    v.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Status.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            if (searchResult.Any())
            {
                return View("GetList", searchResult);
            }
            _notyf.Warning("Voucher không tồn tại");
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Category = await getapiCategory.GetApia("Category");
            return View();
        }

        // POST: VoucherController/Create
        [HttpPost]
        public async Task<IActionResult> Create(Voucher obj)
        {
            try
            {
                var item = getapi.CreateObj(obj, "Voucher").Result;
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
            ViewBag.Category = await getapiCategory.GetApia("Category");
            var lst = getapi.GetApi("Voucher");
            return View(lst.Find(c => c.Id == id));
        }
         

        [HttpPost]
        public async Task<IActionResult> Edit(Voucher obj)
        {
            try
            {
                var item = getapi.UpdateObj(obj, "Voucher").Result;
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

            await getapi.DeleteObj(id, "Voucher");
            return RedirectToAction("GetList");
             
        }
    }
}
