using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ColorController : Controller
    {
        private Getapi<Color> getapi;
        public ColorController()
        {
            getapi = new Getapi<Color>();
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

            return NotFound("Color không tồn tại");
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

                await getapi.CreateObj(obj, "Color");

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

            var lst = getapi.GetApi("Color");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Color obj)
        {
            try
            {
                await getapi.UpdateObj(obj, "Color");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {

            try
            {
               
                if (await getapi.DeleteObj(id, "Color"))
                {
                    var lst = getapi.GetApi("Color").Find(c => c.Id == id);
                    lst.Status = 0;
                    await getapi.UpdateObj(lst, "Color");
                }
                return RedirectToAction("GetList");
            }
            catch (Exception ex)
            {

                _notyf.Error($"Lỗi:{ex.Message} ");
                return View();
            }


        }
    }
}
