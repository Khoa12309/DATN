using _APPAPI.Service;
using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    public class CartController : Controller
    {
        private Getapi<Cart> getapi;
        private Getapi<Product> getapiProduct;
        private Getapi<ProductDetail> getapiPD;
        public CartController()
        {
            getapi = new Getapi<Cart>();
            getapiProduct = new Getapi<Product>();
            getapiPD = new Getapi<ProductDetail>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Cart");
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Cart obj)
        {
            try
            {
                getapi.CreateObj(obj, "Cart");
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

            var lst = getapi.GetApi("Cart");
            return View(lst.Find(c => c.id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Cart obj)
        {
            try
            {
                getapi.UpdateObj(obj, "Cart");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "Cart");
            return RedirectToAction("GetList");

        }
        public async Task<IActionResult> ViewCart(Guid id)
        {
            getapi.DeleteObj(id, "Cart");
            return RedirectToAction("GetList");

        }

        public async Task<IActionResult> AddToCart(Guid id, int soluong)
        {
            var product = getapiPD.GetApi("ProductDetail").Find(c => c.Id == id);
           
            var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");

            if (products.Count == 0)
            {

                products.Add(product);
                SessionService.SetObjToJson(HttpContext.Session, "Cart", products);

            }
            else
            {
                if (!SessionService.CheckProductInCart(id, products)) // SP chưa nằm trong cart
                {
                    products.Add(product);

                    SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
                }
                else
                {
                    var productcart = products.FirstOrDefault(c => c.Id == id);
                    productcart.Quantity += soluong;
                    products.Remove(productcart);
                    products.Add(productcart);
                    SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
                }
            }
            return RedirectToAction("Cart");
        }
        public IActionResult DeleteCartItem(Guid id)
        {
            var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
            var p = products.Find(c => c.Id == id);
            products.Remove(p);
            SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
            return RedirectToAction("Cart");
        }
    }
}
