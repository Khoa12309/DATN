using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    [AllowAnonymous]
    public class AddressController : Controller
    {
        private Getapi<Address> getapi;
        public AddressController()
        {
            getapi = new Getapi<Address>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Address");
            return View(obj);
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
