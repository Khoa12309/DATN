
using APPVIEW.Models;
using Microsoft.AspNetCore.Authorization;


using APPVIEW.Services;

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using APPDATA.Models;
using System.Xml.Linq;

using _APPAPI.Service;

using Microsoft.EntityFrameworkCore;
using _APPAPI.Service;
using Microsoft.VisualBasic;
using APPVIEW.ViewModels;

namespace APPVIEW.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private Getapi<ProductDetail> getapi;
        private Getapi<Category> getapiCategory;
        private Getapi<Color> getapiColor;
        private Getapi<Image> getapiImg;
        private Getapi<Size> getapiSize;
        private Getapi<Supplier> getapiSupplier;
        private Getapi<Product> getapiProduct;
        private Getapi<Material> getapiMaterial;
        private Getapi<Bill> bills;
        private Getapi<BillDetail> billDetails;
        private Getapi<Voucher> getapiVoucher;


        private static readonly Random random = new Random();
        private string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            getapi = new Getapi<ProductDetail>();
            getapiCategory = new Getapi<Category>();
            getapiColor = new Getapi<Color>();
            getapiImg = new Getapi<Image>();
            getapiSize = new Getapi<Size>();
            getapiSupplier = new Getapi<Supplier>();
            getapiProduct = new Getapi<Product>();
            getapiMaterial = new Getapi<Material>();
            bills = new Getapi<Bill>();
            billDetails = new Getapi<BillDetail>();
            getapiVoucher = new Getapi<Voucher>();
        }

        public IActionResult Index()
        {
            var productDetails = getapi.GetApi("ProductDetails");
            var products = getapiProduct.GetApi("Product");

            var productJoin = productDetails.Join(products, ct => ct.Id_Product, s => s.Id, (ct, s) => new { ct, s })
                                    .Select(cs => new { cs.s.Id, cs.s.Name, cs.ct.Price })
                                    .Distinct()
                                    .ToList();

            ViewBag.Result = productJoin;
            return View(productJoin);
        }


        public async Task<IActionResult> Search(string searchTerm)
        {
            var lstProductDetails = getapi.GetApi("ProductDetails").ToList();

            var searchResult = lstProductDetails
                .Where(v =>
                    v.Price.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            if (searchResult.Any())
            {
                return View("Index", searchResult);
            }

            return NotFound("Voucher không tồn tại");

        }

        public IActionResult ViewBill(Guid id)
        {

            var productDetails = getapi.GetApi("ProductDetails");
            var products = getapiProduct.GetApi("Product");

            var productJoin = productDetails.Join(products, ct => ct.Id_Product, s => s.Id, (ct, s) => new { ct, s })
                                    .Select(cs => new { cs.s.Id, cs.s.Name, cs.ct.Price })
                                    .Distinct()
                                    .ToList();
            foreach (var product in productJoin)
            {
                if (product.Id == id)
                {
                    ViewBag.pro = product;

                }
            }

            var prd = getapi.GetApi("ProductDetails").Where(c => c.Id_Product == id).ToList();
            ViewBag.prd = prd;

            var view = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id_Product == id);
            if (view != null)
            {
                ViewBag.chitiet = view;
            }
            ViewBag.id = id;
            ViewBag.size = getapiSize.GetApi("Size");
            ViewBag.color = getapiColor.GetApi("Color");
            return View();
        }

        //[HttpPost]
        //public IActionResult ViewBill(Guid id, Guid size, Guid color)
        //{
        //    var prd = getapi.GetApi("ProductDetails").Where(c => c.Id_Product == id).ToList();
        //    ViewBag.prd = prd;
        //    var view = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id_Product == id);
        //    ViewBag.chitiet = view;
        //    ViewBag.size = getapiSize.GetApi("Size");
        //    ViewBag.color = getapiColor.GetApi("Color");

        //    return View();
        //}



        public string GenerateRandomString(int length)
        {
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Sử dụng:
        // Tạo chuỗi có độ dài 8 ký tự

        public async Task<IActionResult> DatHang(Guid size, Guid color, Guid productId, int soluong, string sdt, string diachi)
        {
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");

            var x = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id_Product == productId && c.Id_Size == size && c.Id_Color == color);

            if (account.Id == Guid.Empty)
            {
                return Redirect("~/Account/Login");
            }
            else if (x == null)
            {
                return BadRequest("Mặt hàng này tạm hết vui lòng chọn size hoặc màu khác");

            }
            else
            {

                var bill = new Bill();

                bill.id = Guid.NewGuid();
                bill.AccountId = account.Id;
                bill.Code = GenerateRandomString(8);
                bill.PhoneNumber = sdt;
                bill.Address = diachi;
                bill.Type = "Online";
                bill.CreateBy = DateTime.Now;
                bill.CreateDate = DateTime.Now;
                bill.UpdateBy = DateTime.Now;
                bill.Status = 1;



                await bills.CreateObj(bill, "Bill");

                var billct = new BillDetail();


                billct.ProductDetailID = x.Id;
                billct.BIllId = bill.id;
                billct.Amount = soluong;
                billct.Price = soluong * x.Price;
                billct.Status = 1;
                await billDetails.CreateObj(billct, "BillDetail");
                bill.TotalMoney = billct.Price;
                await bills.UpdateObj(bill, "Bill");
                ViewBag.Bill = bill;
                ViewBag.Billct = billct;
                ViewBag.sp = getapiProduct.GetApi("Product").FirstOrDefault(c => c.Id == x.Id_Product);
                ViewBag.sizee = getapiSize.GetApi("Size").FirstOrDefault(c => c.Id == x.Id_Size);
                ViewBag.Collor = getapiColor.GetApi("Color").FirstOrDefault(c => c.Id == x.Id_Color);
            }



            return View();
        }
        public IActionResult Thongtin()
        {
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            if (account.Id == Guid.Empty)
            {
                return BadRequest("Bạn chưa đăng nhập");
            }
            else
            {
                var bill = bills.GetApi("Bill").Where(c => c.AccountId == account.Id).ToList();

                foreach (var item in bill)
                {
                    var billct = billDetails.GetApi("BillDetail");
                    foreach (var item2 in billct)
                    {
                        if (item2.BIllId == item.id)
                        {
                            var pr = getapi.GetApi("ProductDetails").Where(c => c.Id == item2.ProductDetailID).ToList();
                            foreach (var item3 in pr)
                            {
                                if (item2.BIllId == item.id) { }

                            }


                        }
                    }
                }
            }
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Shop()
        {
            return View();
        }

        public async Task<IActionResult> Details(Guid id)
        {
            ViewBag.PD = getapi.GetApi("ProductDetails");
            var pro = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id == id);
            ViewBag.lspd = getapi.GetApi("ProductDetails").Where(c => c.Id_Product == pro.Id_Product);
            ViewBag.Img = getapiImg.GetApi("Image");
            ViewBag.Size = getapiSize.GetApi("Size");
            ViewBag.Color = getapiColor.GetApi("Color");
            var a = getapi.GetApi("ProductDetails").GroupBy(c => c.Id_Color).ToList();
            var b = getapi.GetApi("ProductDetails").GroupBy(c => c.Id_Size).ToList();
            foreach (var group in a)
            {

                foreach (var product in b)
                {
                    var lis = group.Intersect(product).ToList();
                }
            }
            var intersect = a.IntersectBy(b, x => x);
            return View(pro);
        }


        public async Task<IActionResult> Checkout()
        {
            // Lấy thông tin voucher từ TempData
            var discountAmountString = TempData["DiscountAmount"] as string;
            var voucherCode = TempData["VoucherCode"] as string;

            ViewBag.Product = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
            var tt = 0;
            foreach (var item in ViewBag.Product)
            {
                tt += (item.Quantity * item.Price);
            }
            if (double.TryParse(discountAmountString, out var discountAmount))
            {
                ViewBag.DiscountAmount = discountAmount;
                ViewBag.TT -= discountAmount;
                ViewBag.VoucherCode = voucherCode;
            }

            ViewBag.TT = tt;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ApplyDiscount()
        {
            // Kiểm tra người dùng đã đăng nhập hay chưa
            //var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            //if (account == null || account.Id == Guid.Empty)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            // Lấy giá trị từ TempData
            // Lấy danh sách các voucher có thể áp dụng

            var vouchers = getapiVoucher.GetApi("Voucher").Where(v => v.EndDate >= DateTime.Now).ToList();
            var view = vouchers.Select(v => new VoucherVm
            {
                Code = v.Code,
                Name = v.Name,
                DiscountAmount = v.DiscountAmount,
                EndDate = v.EndDate,
            }).ToList();
            return View(view);
        }
        [HttpPost]
        public async Task<IActionResult> ApplyDiscount(string selectedVoucher)
        {
            // Lấy thông tin voucher và tính số ngày còn lại
            var voucher = getapiVoucher.GetApi("Voucher").FirstOrDefault(d => d.Code == selectedVoucher && d.EndDate >= DateTime.Now);
            if (voucher != null)
            {
                TempData["DiscountAmount"] = voucher.DiscountAmount.ToString();
                TempData["VoucherCode"] = voucher.Code;

                return RedirectToAction("Checkout");
            }
            else
            {
                // Xử lý khi mã giảm giá không hợp lệ
                ModelState.AddModelError("Error", "Mã giảm giá không hợp lệ");
                return RedirectToAction("Checkout");
            }


        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}