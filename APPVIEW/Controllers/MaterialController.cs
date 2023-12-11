using APPDATA.Models;
using APPVIEW.Services;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MaterialController : Controller
    {
        private Getapi<Material> getapi;
        public INotyfService _notyf;
        public MaterialController(INotyfService notyf)
        {
            _notyf = notyf;
            getapi = new Getapi<Material>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Material");
            return View(obj);
        }
        public async Task<IActionResult> Search(string searchTerm)
        {
            var lstMaterial = getapi.GetApi("Material").ToList();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View("GetList", lstMaterial);
            }
            var searchResult = lstMaterial
                .Where(v =>
                    v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Status.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            if (searchResult.Any())
            {
                return View("GetList", searchResult);
            }

            _notyf.Information("Material không tồn tại");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Material obj)
        {
            try
            {
                var item = getapi.CreateObj(obj, "Material").Result;
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
                _notyf.Error("Lỗi!");
                return View();
            }
        }
        [HttpGet]


        public async Task<IActionResult> Edit(Guid id)
        {

            var lst = getapi.GetApi("Material");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Material obj)
        {
            try
            {
                var item = getapi.UpdateObj(obj, "Material").Result;
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
            try
            {
                if (await getapi.DeleteObj(id, "Material"))
                {
                    var lst = getapi.GetApi("Material").Find(c => c.Id == id);
                    lst.Status = 0;
                    await getapi.UpdateObj(lst, "Material");
                }
                _notyf.Success("Đã xóa");
                return RedirectToAction("GetList");
            }
            catch (Exception ex)
            {
                _notyf.Error($"Lỗi: {ex.Message}");
                return View();
            }

        }
    }
}
