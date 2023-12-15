using APPDATA.Models;
using APPVIEW.Services;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ColorController : Controller
    {
        private Getapi<Color> getapi;
        public INotyfService _notyf;
        public ColorController(INotyfService notyf)
        {
            getapi = new Getapi<Color>();
            _notyf = notyf;
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Color");
            return View(obj);
        }
        public async Task<IActionResult> Search(string searchTerm)
        {
            var lstColor = getapi.GetApi("Color").ToList();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View("GetList", lstColor);
            }
            var searchResult = lstColor
                .Where(v =>
                    v.Colorcode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Status.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            if (searchResult.Any())
            {
                return View("GetList", searchResult);
            }

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Color obj)
        {
            try
            {

                var item = getapi.CreateObj(obj, "Color").Result;
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

            var lst = getapi.GetApi("Color");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Color obj)
        {
            try
            {
                var item = getapi.UpdateObj(obj, "Color").Result;
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
                _notyf.Success("Lỗi!");
                return View();
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {


            try
            {
                if (await getapi.DeleteObj(id, "Color"))
                {
                    _notyf.Success("Chuyển trạng thái thành công");
                    var lst = getapi.GetApi("Color").Find(c => c.Id == id);
                    await getapi.UpdateObj(lst, "Color");
                    _notyf.Success("Xóa thành công");
                }
                return RedirectToAction("GetList");
            }
            catch (Exception)
            {

                _notyf.Error("Lỗi");
                return View();
            }      

        }
    }
}