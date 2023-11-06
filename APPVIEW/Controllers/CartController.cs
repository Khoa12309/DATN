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
        private Getapi<CartDetail> getapiCartD;
        private Getapi<Image> getapiImg;
        private Getapi<Color> getapiColor;
        private Getapi<Size> getapiSize;

        public CartController()
        {
            getapi = new Getapi<Cart>();
            getapiProduct = new Getapi<Product>();
            getapiPD = new Getapi<ProductDetail>();
            getapiCartD = new Getapi<CartDetail>();
            getapiImg = new Getapi<Image>();
            getapiColor = new Getapi<Color>();
            getapiSize = new Getapi<Size>();
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
       

        public async Task<IActionResult> AddToCart(Guid id,int Soluong,Guid color,Guid size)
        {
          
                var account= SessionService.GetUserFromSession(HttpContext.Session, "Account");

            var product = getapiPD.GetApi("ProductDetails").Find(c => c.Id == id);
            if (color!=Guid.Empty&&size!=Guid.Empty)
            {
                product = getapiPD.GetApi("ProductDetails").Find(c => c.Id == id && c.Id_Color == color && c.Id_Size == size);

            }
            product.Quantity = Soluong;
           
            var cart = new Cart()
            {
                id = Guid.NewGuid(),
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Status = true,
                AccountId = account.Id,
            };
            getapi.CreateObj(cart, "Cart");
          
            var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");

            if (products.Count == 0)
            {

                products.Add(product);
                SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
                var cartdetails = new CartDetail()
                {
                    CartId = cart.id,
                    ProductDetail_ID = product.Id,
                     Price=product.Price,
                      Quantity=product.Quantity,
                       
                };
                getapiCartD.CreateObj(cartdetails, "CartDetails");
            }
            else
            {
                if (!SessionService.CheckProductInCart(id, products)) // SP chưa nằm trong cart
                {
                    products.Add(product);

                    SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
                    var cartdetails = new CartDetail()
                    {
                        CartId = cart.id,
                        ProductDetail_ID = product.Id,
                        Price = product.Price,
                        Quantity = product.Quantity,

                    };
                    getapiCartD.CreateObj(cartdetails, "CartDetails");
                }
                else
                {
                    var productcart = products.FirstOrDefault(c => c.Id == id);
                    var productcartdetails = getapiCartD.GetApi("CartDetails").FirstOrDefault(c => c.ProductDetail_ID == id);
                    productcart.Quantity += Soluong;
                    productcartdetails.Quantity+=Soluong;
                    products.Remove(productcart);
                    products.Add(productcart);
                    SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
                    getapiCartD.UpdateObj(productcartdetails, "CartDetails");
                    
                }
            }
            return RedirectToAction("ViewCart");
        }
        public IActionResult DeleteCartItem(Guid id)
        {
            var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
            var productcartdetails = getapiCartD.GetApi("CartDetails").FirstOrDefault(c => c.ProductDetail_ID == id);

            var p = products.Find(c => c.Id == id);
            products.Remove(p);
            SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
            getapiCartD.DeleteObj(productcartdetails.id, "CartDetails");
            return RedirectToAction("ViewCart");
        }
        public async Task<IActionResult> ViewCart()
        {
            //var obj = getapi.GetApi("Cart");
            ViewBag.Img = getapiImg.GetApi("Image");
            ViewBag.Color = getapiColor.GetApi("Color");
            ViewBag.Size = getapiSize.GetApi("Size");
            var product = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
            return View(product);

        }
    }
}
