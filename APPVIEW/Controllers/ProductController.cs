using APPDATA.Models;
using APPVIEW.Services;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private Getapi<Product> getapi;
        public INotyfService _notyf;
        public ProductController(INotyfService notyf)
        {
            _notyf = notyf;
            getapi = new Getapi<Product>();
        }

        public async Task<IActionResult> GetList(int? page)
        {
            var obj = getapi.GetApi("Product");
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(obj.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            var lstProduct = getapi.GetApi("Product").ToList();

            var searchResult = lstProduct
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

            return NotFound("Voucher không tồn tại");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Product obj)
        {
            try
            {
                var item = getapi.CreateObj(obj, "Product").Result;
                if (item != null)
                {
                    _notyf.Success("Thêm thành công!");
                    return RedirectToAction("GetList");
                }
                else
                {
                    _notyf.Warning("Không được để trống");
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

            var lst = getapi.GetApi("Product");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Product obj)
        {
            try
            {
                var item = getapi.UpdateObj(obj, "Product").Result;
                if (item != null)
                {
                    _notyf.Success("Edit Thành công");
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
            await getapi.DeleteObj(id, "Product");
            return RedirectToAction("GetList");

        }
    }
}
