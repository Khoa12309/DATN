using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    public class CartDetailController : Controller
    {
        private Getapi<CartDetail> getapi;
        public CartDetailController()
        {
            getapi = new Getapi<CartDetail>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("CartDetail");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(CartDetail obj)
        {
            try
            {
                getapi.CreateObj(obj, "CartDetail");
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

            var lst = getapi.GetApi("CartDetail");
            return View(lst.Find(c => c.id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(CartDetail obj)
        {
            try
            {
                getapi.UpdateObj(obj, "CartDetail");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "CartDetail");
            return RedirectToAction("GetList");

        }
    }
}
