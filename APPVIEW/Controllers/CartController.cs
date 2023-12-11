using _APPAPI.Service;
using APPDATA.Migrations;
using APPDATA.Models;
using APPVIEW.Services;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;

using System.Net.WebSockets;

namespace APPVIEW.Controllers
{
    public class CartController : Controller
    {
        private readonly INotyfService _notyf;
        private Getapi<Cart> getapi;
        private Getapi<Product> getapiProduct;
        private Getapi<ProductDetail> getapiPD;
        private Getapi<CartDetail> getapiCartD;
        private Getapi<Image> getapiImg;
        private Getapi<Color> getapiColor;
        private Getapi<Size> getapiSize;
        private Getapi<Account> getapiAc;

        public CartController(INotyfService notyf)
        {
            getapi = new Getapi<Cart>();
            getapiProduct = new Getapi<Product>();
            getapiPD = new Getapi<ProductDetail>();
            getapiCartD = new Getapi<CartDetail>();
            getapiImg = new Getapi<Image>();
            getapiColor = new Getapi<Color>();
            getapiSize = new Getapi<Size>();
            getapiAc = new Getapi<Account>();
            _notyf = notyf;
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Cart");
            return View(obj);
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
            int cartCount = cart != null ? cart.Count : 0;
            return Json(new { CartCount = cartCount });
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


        public async Task<IActionResult> AddToCart(Guid id, int Soluong, Guid color, Guid size)
        {
            loadcart();
            if (User.Identity.IsAuthenticated)
            {

                var Uid = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                var acc = getapiAc.GetApi("Account").FirstOrDefault(c => c.Id.ToString() == Uid);
                SessionService.SetObjToJson(HttpContext.Session, "Account", acc);
            }
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var product = getapiPD.GetApi("ProductDetails").Find(c => c.Id == id);
            if (color != Guid.Empty && size != Guid.Empty)
            {
                id = getapiProduct.GetApi("Product").Find(c => c.Id == product.Id_Product).Id;
                product = getapiPD.GetApi("ProductDetails").FirstOrDefault(c => c.Id_Product == id && c.Id_Color == color && c.Id_Size == size);
                if (product==null)
                {

                    _notyf.Warning("Màu hoặc kích thước bạn chọn không còn");
                    TempData["mess"] = "Màu hoặc kích thước bạn chọn không còn";
                    return RedirectToAction("Details", "Home",new { id=id });
                
                }
            }
            product.Quantity = Soluong;
            var cart = getapi.GetApi("Cart").FirstOrDefault(c => c.AccountId == account.Id);
            if (cart == null)
            {

                cart = new Cart()
                {
                    id = Guid.NewGuid(),
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Status = true,
                    AccountId = account.Id,
                };
                getapi.CreateObj(cart, "Cart");
            }        
            var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
            if (products.Count == 0)
            {

                products.Add(product);
                SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
                SessionService.SetObjToJson(HttpContext.Session, "CartDN", products);
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
                    products.Remove(productcart);
                    products.Add(productcart);
                    SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
                    if (productcartdetails!=null)
                    {
                        productcartdetails.Quantity += Soluong;
                        getapiCartD.UpdateObj(productcartdetails, "CartDetails");

                    }

                }
            }
            return Json(new { success = true, count = products.Count });

        }
        public async Task<IActionResult> DeleteCartItem(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {

                var Uid = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                var acc = getapiAc.GetApi("Account").FirstOrDefault(c => c.Id.ToString() == Uid);
                SessionService.SetObjToJson(HttpContext.Session, "Account", acc);
            }
            var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var cart = getapi.GetApi("Cart").FirstOrDefault(c => c.AccountId == account.Id);
            var productcartdetails = getapiCartD.GetApi("CartDetails").FirstOrDefault(c => c.ProductDetail_ID == id&&c.CartId==cart.id);

            var p = products.Find(c => c.Id == id);
            products.Remove(p);
            SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
            if (productcartdetails !=null)
            {
             await   getapiCartD.DeleteObj(productcartdetails.id, "CartDetails");

            }
            return RedirectToAction("ViewCart");
        }
        public async void loadcart()
        {
            var prodDN = SessionService.GetObjFromSession(HttpContext.Session, "CartDN");
            var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
            if (products.Count <1)
            {
                var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
                if (account.Id != Guid.Empty)
                {

                    var cart = getapi.GetApi("Cart").FirstOrDefault(c => c.AccountId == account.Id);
                    if (products.Count != 0)
                    {


                        foreach (var item in products)
                        {
                            var cartdetails = getapiCartD.GetApi("CartDetails").FirstOrDefault(c => c.ProductDetail_ID == item.Id);
                            if (cartdetails != null)
                            {

                                cartdetails.Quantity += item.Quantity;
                                await  getapiCartD.UpdateObj(cartdetails, "CartDetails");

                            }
                            else
                            {
                                cartdetails = new CartDetail()
                                {
                                    CartId = cart.id,
                                    ProductDetail_ID = item.Id,
                                    Price = item.Price,
                                    Quantity = item.Quantity,

                                };
                             await   getapiCartD.CreateObj(cartdetails, "CartDetails");
                            }
                            
                        }
                    }
                    products.Clear();
                    var productcartdetails = getapiCartD.GetApi("CartDetails").Where(c => c.CartId == cart.id);
                    var prod = getapiPD.GetApi("ProductDetails");
                    foreach (var item in productcartdetails)
                    {
                        var PD = prod.Find(c => c.Id == item.ProductDetail_ID);
                        PD.Quantity = item.Quantity;
                        products.Add(PD);
                    }

                   
                    SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
                   


                }
            }
        }
        public async Task<IActionResult> ViewCart()
        {

            loadcart();
            ViewBag.Img = getapiImg.GetApi("Image");
            ViewBag.prod =getapiPD.GetApi("ProductDetails");
            ViewBag.Color = getapiColor.GetApi("Color");
            ViewBag.Size = getapiSize.GetApi("Size");
           
            var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
            double tiensp =0;
            foreach (var item in products)
            {

                tiensp += (double)item.Quantity * item.Price;
            }
            ViewBag.tt = tiensp; 
            return View(products);

        }
        [HttpPost]
        public async Task<IActionResult> UpdateCart (List<APPDATA.Models.ProductDetail> obj)
        {
            foreach (var item in obj)
            {

                var product = getapiPD.GetApi("ProductDetails").FirstOrDefault(c => c.Id_Product == item.Id_Product && c.Id_Color == item.Id_Color && c.Id_Size == item.Id_Size);
                if (product == null)
                {

                  
                    TempData["mess"] = "Sản Phẩm " + item.Name + " không còn màu hoặc kích thước bạn chọn ";
                    return RedirectToAction("viewcart");
                }
                if (product.Quantity<item.Quantity)
                {
                    TempData["mess"] ="Sản Phẩm "+ item.Name + " chỉ còn  " + item.Quantity;
                    return RedirectToAction("viewcart");
                }


                var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
                var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
                var cart = getapi.GetApi("Cart").FirstOrDefault(c => c.AccountId == account.Id);
                var productcartdetails = getapiCartD.GetApi("CartDetails").FirstOrDefault(c => c.ProductDetail_ID == item.Id&& c.CartId == cart.id);
                var p = products.Find(c => c.Id == item.Id);
                products.Remove(p);
                SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
                if (productcartdetails!=null)
                {
                  await  getapiCartD.DeleteObj(productcartdetails.id, "CartDetails");
                }
               
                if (!SessionService.CheckProductInCart(product.Id, products)) // SP chưa nằm trong cart
                    {
                    product.Quantity = item.Quantity;
                        products.Add(product);

                        SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
                    if (cart != null)
                    {
                        var cartdetails = new CartDetail()
                        {
                            CartId = cart.id,
                            ProductDetail_ID = product.Id,
                            Price = product.Price,
                            Quantity = product.Quantity,

                        };
                        getapiCartD.CreateObj(cartdetails, "CartDetails");
                    }                     
                    }
                    else
                    {
                        var productcart = products.FirstOrDefault(c => c.Id == product.Id);
                        var productcds = getapiCartD.GetApi("CartDetails").FirstOrDefault(c => c.ProductDetail_ID == product.Id);
                        productcart.Quantity += item.Quantity;
                        products.Remove(productcart);
                        products.Add(productcart);
                        SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
                        if (productcds != null)
                        {
                         productcds.Quantity += item.Quantity;
                            getapiCartD.UpdateObj(productcds, "CartDetails");

                        }
                   }
            }
            return RedirectToAction("ViewCart");

        }
        
        public async Task<JsonResult> Chossesize([FromBody] ProductDetail id)
        {
            var color = getapiPD.GetApi("ProductDetails").Where(c=>c.Id_Size==id.Id_Size&&c.Id==id.Id);
            return Json(color);
        }
    }
}
