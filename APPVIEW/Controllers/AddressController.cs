using APPDATA.Models;
using APPVIEW.Services;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AddressController : Controller
    {
        private Getapi<Address> getapi;
        public INotyfService _notyf;
        public AddressController(INotyfService notyf)
        {
            getapi = new Getapi<Address>();
            _notyf = notyf;
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Address");
            return View(obj);
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            var lstAddress = getapi.GetApi("Address").ToList();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View("GetList", lstAddress);
            }
            var searchResult = lstAddress
                .Where(v =>
                    v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.SpecificAddress.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Ward.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.District.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Province.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.DefaultAddress.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Status.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            if (searchResult.Any())
            {
                return View("GetList", searchResult);
            }

            _notyf.Warning("Địa chỉ không tồn tại");
            return View("GetList");
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Address obj)
        {
            try
            {
                getapi.CreateObj(obj, "Address");
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

            var lst = getapi.GetApi("Address");
            return View(lst.Find(c => c.id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Address obj)
        {
            try
            {
               
                getapi.UpdateObj(obj, "Address");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "Address");
            return RedirectToAction("GetList");

        }
    }
}
