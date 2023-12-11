
using APPVIEW.Models;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreHero.ToastNotification.Abstractions;
using APPVIEW.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using APPDATA.Models;
using System.Xml.Linq;
using _APPAPI.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using APPVIEW.ViewModels;
using _APPAPI.ViewModels;
using System;
using System.Configuration;
using static APPVIEW.ViewModels.FilterData;
using Org.BouncyCastle.Tsp;
using AspNetCore;
using System.Reflection.Metadata;
using System.Security.Principal;
using DocumentFormat.OpenXml.Math;

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
        private Getapi<Address> getapiAddress;
        private Getapi<Account> getapiAc;
        private Getapi<CartDetail> getapiCD;
        private Getapi<PaymentMethodDetail> getapiPMD;
        private Getapi<PaymentMethod> getapiPM;
        private Getapi<VoucherForAcc> getapiVoucherAcc;
        private Getapi<Role> getapiRole;
        public INotyfService _notyf;

        private static readonly Random random = new Random();
        private string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public HomeController(ILogger<HomeController> logger,INotyfService notyf)
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
            getapiAddress = new Getapi<Address>();
            getapiAc = new Getapi<Account>();

            getapiCD = new Getapi<CartDetail>();
            getapiPM = new Getapi<PaymentMethod>();
            getapiRole= new Getapi<Role>();
            getapiPMD = new Getapi<PaymentMethodDetail>();
            getapiVoucherAcc = new Getapi<VoucherForAcc>();

            _notyf = notyf;


        }

        public async Task<IActionResult> Index()
        {

            if (User.Identity.IsAuthenticated)
            {

                var Uid = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                var acc = getapiAc.GetApi("Account").FirstOrDefault(c => c.Id.ToString() == Uid);
                SessionService.SetObjToJson(HttpContext.Session, "Account", acc);
            }

            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
           
            if (account.Id == Guid.Empty)
            {
               
                account = getapiAc.GetApi("Account").FirstOrDefault(c => c.Name == "khach k dang nhap");

                await getapiAc.CreateObj(account, "Account");
                if (account == null)
                {
                    account = new Account();
                    account.Id = Guid.Empty;
                    account.Status = 1;
                    account.Name = "khach k dang nhap";
                    account.Email = "";
                    account.Password = "";
                    account.Avatar = "";
                    account.Create_date = DateTime.Now;
                    account.Update_date = DateTime.Now;
                    account.IdRole = getapiRole.GetApi("Role").FirstOrDefault(c => c.name == "Customer").id;
                    await getapiAc.CreateObj(account, "Account");
                }
            }
            var productDetails = getapi.GetApi("ProductDetails").Where(c => c.Status == 1 && c.Quantity > 0);
            var products = getapiProduct.GetApi("Product");

            var productJoin = productDetails.Join(products, ct => ct.Id_Product, s => s.Id, (ct, s) => new { ct, s })
                                    .Select(cs => new { cs.ct.Id_Product, cs.s.Name, cs.ct.Price, cs.ct.Id })
                                    .Distinct()
                                    .ToList();
            ViewBag.Result = productJoin;

            ViewBag.Img = getapiImg.GetApi("Image");

            return View(productJoin);
        }
        [HttpGet]
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

        public async Task<IActionResult> ViewBill(Guid id)
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
            var prdct = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id_Product == id);
            
            if (prdct != null)
            {
                ViewBag.image = getapiImg.GetApi("Image").FirstOrDefault(c => c.IdProductdetail == prdct.Id);
            }
            else
            {
                ViewBag.image = null;
            }
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");

            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetProvincesAsync();

            //Kiểm tra kết quả trả về
            if (response.Code == 200) // Thành công
            {
                // Trả về danh sách các quận/huyện dưới dạng JSON
                ViewBag.province = response.Data;
            }

            ViewBag.Product = SessionService.GetObjFromSession(HttpContext.Session, "Cart");

            var discountAmountString = TempData["DiscountAmount"] as string;
            var voucherCode = TempData["VoucherCode"] as string;
            var valueString = TempData["Value"] as string;

            var tt = 0;
            foreach (var item in ViewBag.Product)
            {
                tt += (item.Quantity * item.Price);
            }
            if (!string.IsNullOrEmpty(voucherCode))
            {

                if (double.TryParse(valueString, out var percentValue) && double.TryParse(discountAmountString, out var discountAmount))
                {
                    percentValue /= 100;

                    var discountValue = tt * percentValue;

                    discountValue = Math.Max(discountValue, 0);

                    var total = (double)tt - discountValue;

                    ViewBag.FirstDiscountAmount = discountAmount;
                    ViewBag.DiscountAmount = discountValue;
                    ViewBag.FirstValue = valueString;
                    ViewBag.Value = percentValue;
                    ViewBag.VoucherCode = voucherCode;

                    ViewBag.TT = total;
                }
            }
            ViewBag.TT = tt;
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var can = 100;
            ViewBag.huyen = 0;
            ViewBag.xa = 0;

            if (account != null)
            {
                var dc = getapiAddress.GetApi("Address").FirstOrDefault(c => c.AccountId == account.Id);
                if (dc != null)
                {
                    var p = await province(dc.Province);
                    if (p != 0)
                    {

                        var d = await dis(dc.District, p);
                        if (d != 0)
                        {
                            var w = await wad(dc.Ward, d);
                            int sship = await getServiceShip(d);

                            client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee?service_id={sship}" + $"&insurance_value=100000&to_ward_code={w}" + $"&to_district_id={d}" + "&from_district_id=3440" + $"&weight={can}", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");

                            // Gọi API để lấy danh sách các tỉnh/thành phố
                            ViewBag.tinh = p;
                            ViewBag.huyen = d;
                            ViewBag.xa = w;
                            var fee = await client.GetFeeshipAsync();

                            //Kiểm tra kết quả trả về
                            if (fee.Code == 200) // Thành công
                            {
                                // Trả về danh sách các quận/huyện dưới dạng JSON
                                ViewBag.fee = fee.Data.total;
                            }
                        }

                    }


                }
                else
                {
                    ViewBag.fee = 0;
                }

                return View(dc);
            }
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

        public async Task<IActionResult> Nhanhang(Guid id)
        {
            var x = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);
            x.Status = 4;
            x.PayDate = DateTime.Now;
            await bills.UpdateObj(x, "Bill");
            return RedirectToAction("Thongtin");
        }
        public async Task<IActionResult> HuyDon(Guid id)
        {
            var x = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);
            x.Status = 0;
            await bills.UpdateObj(x, "Bill");
            return RedirectToAction("Thongtin");
        }
        public async Task<IActionResult> UpdateAddress(Guid id)
        {


            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");


            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");

            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetProvincesAsync();

            //Kiểm tra kết quả trả về
            if (response.Code == 200) // Thành công
            {
                // Trả về danh sách các quận/huyện dưới dạng JSON
                ViewBag.province = response.Data;
            }

            ViewBag.Product = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
            var tt = 0;
            foreach (var item in ViewBag.Product)
            {
                tt += (item.Quantity * item.Price);
            }
            ViewBag.TT = tt;
            var x = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);
            ViewBag.bill = x;

            return View(x);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAddress(Guid id, string sdt, int province, string district, string ward, string diachict, float ship)
        {
            string province2 = "";
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");


            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");

            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetProvincesAsync();

            //Kiểm tra kết quả trả về
            if (response.Code == 200) // Thành công
            {
                // Trả về danh sách các quận/huyện dưới dạng JSON
                ViewBag.province = response.Data;
            }

            foreach (var item in response.Data)
            {
                if (item.ProvinceID == province)
                {

                    province2 = item.ProvinceName; break;
                }
            }

            // Gọi API để lấy danh sách các tỉnh/thành phố

            var diachi = diachict + "-" + ward + "-" + district + "-Tỉnh " + province2;

            var x = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);
            x.PhoneNumber = sdt;
            x.Address = diachi;
            x.ShipFee = ship;
            await bills.UpdateObj(x, "Bill");
            return RedirectToAction("Thongtin");
        }

        // Sử dụng:
        // Tạo chuỗi có độ dài 8 ký tự


        public async Task<IActionResult> DatHangN(Address obj, string pay, float phiship, float voucher, string vouchercode)
        {
            if (obj.Name==null)
            {
                _notyf.Warning("Tên không được để trống");
                return RedirectToAction("checkout");
            } 
            if (obj.PhoneNumber==null)
            {
                _notyf.Warning("Số điện thoại không được để trống");
                return RedirectToAction("checkout");
            }  
            if (obj.Province==null)
            {
                _notyf.Warning("Thành phố không được để trống");
                return RedirectToAction("checkout");
            } 
            if (obj.District==null)
            {
                _notyf.Warning("Quận/huyện không được để trống");
                return RedirectToAction("checkout");
            }
            if (obj.Ward==null)
            {
                _notyf.Warning("Phường/xã không được để trống");
                return RedirectToAction("checkout");
            } 

            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            if (account.Id == Guid.Empty)
            {
                account = getapiAc.GetApi("Account").FirstOrDefault(c => c.Name == "khach k dang nhap");
              
            }
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");



            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetProvincesAsync();
            foreach (var item in response.Data)
            {
                if (item.ProvinceID.ToString() == obj.Province)
                {
                    obj.Province = item.ProvinceName; break;
                }
            }
            var bill = new Bill();
            bill.id = Guid.NewGuid();
            bill.Name = obj.Name;
            bill.AccountId = account.Id;
            bill.Code = GenerateRandomString(8);
            bill.PhoneNumber = obj.PhoneNumber;
            bill.Address = obj.Province + "-" + obj.District + "-" + obj.Ward + "-" + obj.SpecificAddress;
            bill.CreateBy = DateTime.Now;
            bill.CreateDate = DateTime.Now;
            bill.UpdateBy = DateTime.Now;
            bill.ShipFee = phiship;
            bill.PayDate = DateTime.Now;
            bill.TotalMoney = 0;
            bill.Status = 1;
            bill.PayDate = DateTime.Now;
            bill.Type = pay + " - Chưa Thanh Toán ";
            var vo = getapiVoucher.GetApi("Voucher").FirstOrDefault(c => c.Code == vouchercode);
            if (vo != null)
            {
                bill.Voucherid = vo.Id;
            }


            await bills.CreateObj(bill, "Bill");

            var procarrt = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
            if (procarrt != null)
            {
                foreach (var item in procarrt)
                {
                    var billct = new BillDetail();
                    billct.ProductDetailID = item.Id;
                    billct.BIllId = bill.id;
                    billct.Amount = item.Quantity;
                    billct.Price = item.Quantity * item.Price;
                    billct.Status = 1;
                    await billDetails.CreateObj(billct, "BillDetail");
                    bill.TotalMoney += billct.Price;
                    await bills.UpdateObj(bill, "Bill");
                }
            }
            var voucherAcc = getapiVoucherAcc.GetApi("VoucherForAcc").FirstOrDefault(c => c.Id_Account == account.Id && c.Id_Voucher == bill.Voucherid);

            if (voucherAcc != null && voucherAcc.Id_Account == account.Id)
            {
                if (voucherAcc.Value > 0) // Kiểm tra nếu voucher có giảm giá theo phần trăm
                {
                    // Tính toán giảm giá dựa trên phần trăm
                    float percentage = voucherAcc.Value / 100f;
                    //float? discountFromPercentage = bill.TotalMoney * percentage;
                    var discount = bill.TotalMoney * percentage;
                    if (discount > voucherAcc.DiscountAmount)
                    {
                        bill.TotalMoney = (float)(bill.TotalMoney - voucherAcc.DiscountAmount) + bill.ShipFee;
                    }
                    else
                    {
                        bill.TotalMoney = (bill.TotalMoney - discount) + bill.ShipFee;
                    }
                    voucherAcc.Status = 2; file:///C:/Program%20Files%20(x86)/UltraViewer/images/close-icon.png
                    await getapiVoucherAcc.UpdateObj(voucherAcc, "VoucherForAcc");


                }
                await bills.UpdateObj(bill, "Bill");
            }
            else
            {
                bill.TotalMoney += bill.ShipFee;
                await bills.UpdateObj(bill, "Bill");
            }



            if (pay == "Online")
            {

                return await Payment(bill);
            }
            else
            {
                var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");


                foreach (var item in products)
                {
                    var productcartdetails = getapiCD.GetApi("CartDetails").FirstOrDefault(c => c.ProductDetail_ID == item.Id);

                    var p = products.Find(c => c.Id == item.Id);

                    if (productcartdetails != null)
                    {
                        await getapiCD.DeleteObj(productcartdetails.id, "CartDetails");

                    }
                }
                products.Clear();
                SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
                if (account.Name== "khach k dang nhap")
                {
                    _notyf.Success("Đặt hàng thành công");
                    return RedirectToAction("Index");
                }
                _notyf.Success("Đặt hàng thành công");

                return RedirectToAction("Thongtin");
            }

        }


        [HttpPost]
        public ActionResult getsl(string productId, string size, string color)
        {
            int sl = 0;
            var foundSanPhamChiTiet = getapi.GetApi("ProductDetails")
                .FirstOrDefault(c => c.Id_Product == Guid.Parse(productId) && c.Id_Size == Guid.Parse(size) && c.Id_Color == Guid.Parse(color));

            if (foundSanPhamChiTiet != null)
            {
                var img = getapiImg.GetApi("Image").FirstOrDefault(c => c.IdProductdetail == foundSanPhamChiTiet.Id);
                return Json(new { success = true, idsanphamcthitiet = foundSanPhamChiTiet, img = img });
            }
            else
            {
                return Json(new { success = true, idsanphamcthitiet = sl });
            }
        }



        public ActionResult getPrdDetails(string productId, string size, string color)
        {


            // Debug để kiểm tra dữ liệu được nhận từ AJAX request
            Console.WriteLine($"Received: ProductId={productId}, Size={size}, Color={color}");

            var idsanphamcthitiet = getapi.GetApi("ProductDetails")
                .FirstOrDefault(c => c.Id_Product == Guid.Parse(productId) && c.Id_Size == Guid.Parse(size) && c.Id_Color == Guid.Parse(color));



            // Trả về dữ liệu dưới dạng JSON
            return Json(new { success = true, idsanphamcthitiet = idsanphamcthitiet });
        }

        public async Task<IActionResult> DatHang(Guid size, Guid color, Guid productId, int soluong, string sdt, float ship, int province, string district, string ward, string diachict, string pay)

        {

            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var obj = getapiAddress.GetApi("Address").FirstOrDefault(c => c.AccountId == account.Id);

            if (account.Id == Guid.Empty)
            {
                return Redirect("~/Account/Login");
            }


            var x = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id_Product == productId && c.Id_Size == size && c.Id_Color == color);
            if (x == null)
            {
                return BadRequest("Mặt hàng này tạm hết vui lòng chọn size hoặc màu khác");

            }
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");

            string province2 = "";

            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetProvincesAsync();

            foreach (var item in response.Data)
            {

                if (item.ProvinceID == province)
                {

                    province2 = item.ProvinceName; break;
                }
            }
            var diachi = diachict + "-" + ward + "-" + district + "-Tỉnh " + province2;
            var bill = new Bill();
            bill.id = Guid.NewGuid();
            bill.AccountId = account.Id;
            bill.Code = GenerateRandomString(8);
            bill.PhoneNumber = obj.PhoneNumber;
            bill.Address = diachi;
            bill.CreateBy = DateTime.Now;
            bill.CreateDate = DateTime.Now;
            bill.UpdateBy = DateTime.Now;
            bill.ShipFee = ship;
            bill.PayDate = DateTime.Now;
            bill.TotalMoney =
            bill.Status = 1;
            bill.PayDate = DateTime.Now;
            bill.Type = pay + " - Chưa Thanh Toán ";
            //var vo = getapiVoucher.GetApi("Voucher").FirstOrDefault(c => c.Code == vouchercode);
            //if (vo != null)
            //{
            //    bill.Voucherid = vo.Id;
            //}


            await bills.CreateObj(bill, "Bill");
            if (x != null)

            {
                var billct = new BillDetail();
                billct.ProductDetailID = x.Id;
                billct.BIllId = bill.id;
                billct.Amount = soluong;
                billct.Price = soluong * x.Price;
                billct.Status = 1;
                await billDetails.CreateObj(billct, "BillDetail");
                bill.TotalMoney = billct.Price + ship;
                await bills.UpdateObj(bill, "Bill");

            }

            var voucherAcc = getapiVoucherAcc.GetApi("VoucherForAcc").FirstOrDefault(c => c.Id_Account == account.Id && c.Id_Voucher == bill.Voucherid);

            if (voucherAcc != null && voucherAcc.Id_Account == account.Id)
            {
                if (voucherAcc.Value > 0) // Kiểm tra nếu voucher có giảm giá theo phần trăm
                {
                    // Tính toán giảm giá dựa trên phần trăm
                    float percentage = voucherAcc.Value / 100f;
                    //float? discountFromPercentage = bill.TotalMoney * percentage;
                    var discount = bill.TotalMoney * percentage;
                    if (discount > voucherAcc.DiscountAmount)
                    {
                        bill.TotalMoney = (float)(bill.TotalMoney - voucherAcc.DiscountAmount) + bill.ShipFee;
                    }
                    else
                    {
                        bill.TotalMoney = (bill.TotalMoney - discount) + bill.ShipFee;
                    }
                    voucherAcc.Status = 2; /* file:///C:/Program%20Files%20(x86)/UltraViewer/images/close-icon.png */
                    await getapiVoucherAcc.UpdateObj(voucherAcc, "VoucherForAcc");


                }
                await bills.UpdateObj(bill, "Bill");
            }
            else
            {
                bill.TotalMoney += bill.ShipFee;
                await bills.UpdateObj(bill, "Bill");
            }


            if (pay == "Online")
            {

                return await Payment(bill);
            }
            else
            {

                return RedirectToAction("Thongtin");
            }

        }

        public async Task<IActionResult> Thongtin()
        {
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var userBills = bills.GetApi("Bill").Where(c => c.AccountId == account.Id && c.Status != 4 && c.Status != 5).OrderByDescending(d => d.CreateDate).ToList();
            ViewBag.viewbill = userBills;

            if (User.Identity.IsAuthenticated)
            {

                var Uid = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                var acc = getapiAc.GetApi("Account").FirstOrDefault(c => c.Id.ToString() == Uid);
                SessionService.SetObjToJson(HttpContext.Session, "Account", acc);
            }
            else
            {
                return Redirect("~/Account/login");
            }
            var billss = bills.GetApi("Bill").Where(c => c.Type == "Online - Chưa Thanh Toán ").ToList();
            foreach (var item in billss)
            {

                var xx = bills.GetApi("Bill").FirstOrDefault(c => c.id == item.id);
                var y = billDetails.GetApi("BillDetail").Where(c => c.BIllId == item.id).ToList();
                foreach (var item2 in y)
                {
                    await billDetails.DeleteObj(item2.id, "BillDetail");
                }
                await bills.DeleteObj(item.id, "Bill");

            }
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetProvincesAsync();
            var billDetailsApi = billDetails.GetApi("BillDetail");
            var productDetailsApi = getapi.GetApi("ProductDetails");
            var productsApi = getapiProduct.GetApi("Product");
            var voucherApi = getapiVoucher.GetApi("Voucher");
            // Lấy thông tin về voucher
            foreach (var userBill in userBills)
            {
                if (userBill.Voucherid.HasValue)
                {
                    userBill.Voucher = voucherApi.FirstOrDefault(v => v.Id == userBill.Voucherid);
                }
            }
            ViewBag.viewbillct = billDetailsApi;
            ViewBag.viewprdct = productDetailsApi;
            ViewBag.viewprd = productsApi;
            ViewBag.Voucher = voucherApi;
            ViewBag.sizee = getapiSize.GetApi("Size");
            ViewBag.Collor = getapiColor.GetApi("Color");
            ViewBag.image = getapiImg.GetApi("Image");
            return View(userBills);

        }

        [Authorize(Roles = "Customer")]
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Shop(string sortOrder)
        {
            var img = getapiImg.GetApi("Image");
            var productDetails = getapi.GetApi("ProductDetails").Where(c => c.Status == 1 && c.Quantity > 0).ToList();
            ViewBag.size = getapiSize.GetApi("Size");
            ViewBag.color = getapiColor.GetApi("Color");
            try
            {
                var productsWithImages = productDetails
                    .Join(img, pd => pd.Id, pi => pi.IdProductdetail, (pd, pi) => new { ProductDetail = pd, Image = pi })
                    .ToList();

                // Sắp xếp danh sách sản phẩm tùy thuộc vào tham số sortOrder
                switch (sortOrder)
                {
                    case "asc":
                        productsWithImages = productsWithImages.OrderBy(item => item.ProductDetail.Price).ToList();
                        break;
                    case "desc":
                        productsWithImages = productsWithImages.OrderByDescending(item => item.ProductDetail.Price).ToList();
                        break;
                    default:
                        if (sortOrder != null)
                        {
                            productsWithImages = productsWithImages.Where(c => c.ProductDetail.Name.ToLower().Contains(sortOrder.ToLower())).ToList();
                        }

                        break;
                }

                ViewBag.Products = productsWithImages;
            }
            catch (Exception ex)
            {
                // Sử dụng logging để ghi lại lỗi
                _logger.LogError($"Error in Shop action: {ex.Message}");
            }

            return View();
        }

        [HttpPost]
        public IActionResult GetFilteredProducts([FromBody] FilterData filter)
        {
            try
            {
                if (filter == null)
                {
                    return BadRequest("Invalid filter data");
                }

                var img = getapiImg.GetApi("Image");
                var filterProductsWithImages = getapi.GetApi("ProductDetails")
                    .Join(img, pd => pd.Id, pi => pi.IdProductdetail, (pd, pi) => new { ProductDetail = pd, Image = pi })
                    .ToList();
                //var filterProductsWithImages = getapi.GetApi("ProductDetails")
                //   .Join(img, pd => pd.Id, pi => pi.IdProductdetail, (pd, pi) => new { ProductDetail = pd, Image = pi })
                //   .Select(cs => new { cs.ProductDetail.Id, cs.Image.Name, cs.ProductDetail.Id_Product, cs.ProductDetail.Price, cs.ProductDetail.Id_Color, cs.ProductDetail.Id_Size, nap = cs.ProductDetail.Name })
                //   .ToList();
                //Console.WriteLine("Original Products Count: " + filterProductsWithImages.Count);

                // Áp dụng bộ lọc
                if (filter.Colors != null && filter.Colors.Count > 0 && !filter.Colors.Contains("all"))
                {
                    filterProductsWithImages = filterProductsWithImages
                        .Where(p => filter.Colors.Contains(p.ProductDetail.Id_Color.ToString())).ToList();

                }

                if (filter.Sizes != null && filter.Sizes.Count > 0 && !filter.Sizes.Contains("all"))
                {
                    filterProductsWithImages = filterProductsWithImages
                        .Where(p => filter.Sizes.Contains(p.ProductDetail.Id_Size.ToString()))
                        .ToList();

                    //Console.WriteLine("Filtered Products Count: " + filterProductsWithImages.Count);
                    //Console.WriteLine("Selected Sizes: " + string.Join(", ", filter.Sizes));
                    //foreach (var product in filterProductsWithImages)
                    //{
                    //    var sizeName = product.ProductDetail.Size?.Name ?? "null";
                    //}
                }

                if (filter.PriceRanges != null && filter.PriceRanges.Count > 0 && !filter.PriceRanges.Contains("all"))
                {
                    List<PriceRange> priceRanges = new List<PriceRange>();
                    foreach (var range in filter.PriceRanges)
                    {
                        var value = range.Split("-").ToArray();
                        if (value.Length == 2)
                        {
                            PriceRange priceRange = new PriceRange();
                            if (Int32.TryParse(value[0], out int minValue) && Int32.TryParse(value[1], out int maxValue))
                            {
                                priceRange.Min = minValue;
                                priceRange.Max = maxValue;
                                priceRanges.Add(priceRange);
                            }
                        }
                    }

                    filterProductsWithImages = filterProductsWithImages
                        .Where(p => priceRanges.Any(r => p.ProductDetail.Price >= r.Min && p.ProductDetail.Price <= r.Max))
                        .ToList();
                }

                ViewBag.Products = filterProductsWithImages;

                return PartialView("_ReturnProducts", filterProductsWithImages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal Server Error", message = ex.Message });
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            ViewBag.PD = getapi.GetApi("ProductDetails");
            var pro = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id == id);
            ViewBag.lspd = getapi.GetApi("ProductDetails").Where(c => c.Id_Product == pro.Id_Product);
            ViewBag.Img = getapiImg.GetApi("Image");
            ViewBag.Size = getapiSize.GetApi("Size");
            ViewBag.Color = getapiColor.GetApi("Color");

            var voucherList = getapiVoucher.GetApi("Voucher");
            if (voucherList != null)
            {
                ViewBag.Voucher = voucherList.Where(v => v.EndDate >= DateTime.Now && v.Quantity > 0 && v.Status == 1).ToList();
            }


            ViewBag.Category = getapiCategory.GetApi("Category");
            ViewBag.Supplier = getapiSupplier.GetApi("Supplier");
            ViewBag.Material = getapiMaterial.GetApi("Material");

            return View(pro);
        }



        [HttpPost]
        public async Task<JsonResult> district([FromBody] int provinceid)
        {

            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/district?province_id={provinceid}", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetDistricsAsync();
            //Kiểm tra kết quả trả về
            if (response.Code == 200) // Thành công
            {
                // Trả về danh sách các quận/huyện dưới dạng JSON
                return Json(response.Data);
            }
            else // Thất bại
            {
                // Trả về thông báo lỗi
                return Json(response.Data);
            }
        }
        [HttpPost]
        public async Task<JsonResult> ward([FromBody] int districtid)
        {
            getServiceShip(districtid);
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id={districtid}", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetWardsAsync();
            //Kiểm tra kết quả trả về
            if (response.Code == 200) // Thành công
            {
                // Trả về danh sách các quận/huyện dưới dạng JSON
                return Json(response.Data);
            }
            else // Thất bại
            {
                // Trả về thông báo lỗi
                return Json(response.Data);
            }
        }

        [HttpPost]
        public async Task<JsonResult> feeship([FromBody] diachi data)
        {
            var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
            var can = 100;
            if (products.Count != 0)
            {
                var sl = 0;
                foreach (var item in products)
                {
                    sl += item.Quantity;
                }
                can = sl * 100;
            }
            int sship = await getServiceShip(data.to_district_id);
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee?service_id={sship}" + $"&insurance_value=100000&to_ward_code={data.towardcode.ToString()}" + $"&to_district_id={data.to_district_id.ToString()}" + "&from_district_id=3440" + $"&weight={can}", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetFeeshipAsync();
            // Kiểm tra kết quả trả về
            if (response.Code == 200) // Thành công
            {
                // Trả về danh sách các quận/huyện dưới dạng JSON
                return Json(response.Data);
            }
            else // Thất bại
            {
                // Trả về thông báo lỗi
                return Json(response.Data);

            }

        }
        [HttpPost]
        public async Task<JsonResult> feeship2(int soluong, int towardcode, int to_district_id)
        {

            var can = 100;

            if (soluong != null || soluong != 0)
            {

                can = soluong * 100;
            }

            int sship = await getServiceShip(to_district_id);
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee?service_id={sship}" + $"&insurance_value=100000&to_ward_code={towardcode.ToString()}" + $"&to_district_id={to_district_id.ToString()}" + "&from_district_id=3440" + $"&weight={can}", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetFeeshipAsync();
            // Kiểm tra kết quả trả về
            if (response.Code == 200) // Thành công
            {
                // Trả về danh sách các quận/huyện dưới dạng JSON
                return Json(response.Data);
            }
            else // Thất bại
            {
                // Trả về thông báo lỗi
                return Json(response.Data);
            }

        }
        public async Task<int> getServiceShip(int data)
        {
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/available-services?from_district=3440&shop_id=4189096&to_district={data}", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetServiceAsync();
            // Kiểm tra kết quả trả về
            if (response.Code == 200) // Thành công
            {
                //  sship= response.Data[0].service_id;
                // Trả về danh sách các quận/huyện dưới dạng JSON
                return response.Data[0].service_id;
            }
            else // Thất bại
            {
                // Trả về thông báo lỗi
                return 0;
            }

        }

        public async Task<int> province(string ten)
        {
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetProvincesAsync();
            //Kiểm tra kết quả trả về
            if (response.Code == 200) // Thành công
            {
                // Trả về danh sách các quận/huyện dưới dạng JSON

                foreach (var item in response.Data)
                {
                    if (ten.ToLower() == item.ProvinceName.ToLower())
                    {
                        return item.ProvinceID;
                    }
                }
            }
            return 0;


        }
        public async Task<int> dis(string ten, int id)
        {
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/district?province_id={id}", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetDistricsAsync();
            //Kiểm tra kết quả trả về
            if (response.Code == 200) // Thành công
            {

                // Trả về danh sách các quận/huyện dưới dạng JSON

                foreach (var item in response.Data)
                {
                    if (item.NameExtension.Any(c => c.Contains(ten))||item.DistrictName.ToLower().Contains(ten.ToLower()))
                    {
                        return item.DistrictID;
                    }
                }
            }
            return 0;


        }
        public async Task<int> wad(string ten, int id)
        {
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id={id}", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetWardsAsync();
            //Kiểm tra kết quả trả về
            if (response.Code == 200) // Thành công
            {

                // Trả về danh sách các quận/huyện dưới dạng JSON

                foreach (var item in response.Data)
                {
                    if (item.NameExtension.Any(c => c.Contains(ten)) || item.WardName.ToLower() == ten.ToLower())
                    {
                        return item.WardCode;
                    }
                }
            }
            return 0;


        }
        public async Task<IActionResult> CheckoutOnl(Guid id)
        {
            ViewBag.id = id;
            var prd = getapi.GetApi("ProductDetails").Where(c => c.Id_Product == id).ToList();
            ViewBag.prd = prd;
            ViewBag.size = getapiSize.GetApi("Size");
            ViewBag.color = getapiColor.GetApi("Color");
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");

            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetProvincesAsync();
            if (response.Code == 200) // Thành công
            {
                // Trả về danh sách các quận/huyện dưới dạng JSON
                ViewBag.province = response.Data;
            }

            //// fee ship
            var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
            var can = 100;


            ViewBag.Product = products;


            var tt = 0;
            // Lấy thông tin voucher từ TempData
            var discountAmountString = TempData["DiscountAmount"] as string;
            var voucherCode = TempData["VoucherCode"] as string;
            var valueString = TempData["Value"] as string;
            var vocher = getapiVoucher.GetApi("Voucher").FirstOrDefault(c => c.Code == voucherCode);
            ViewBag.vocher = vocher;
            foreach (var item in ViewBag.Product)
            {

                tt += (item.Quantity * item.Price);
            }
            if (!string.IsNullOrEmpty(voucherCode))
            {

                if (double.TryParse(valueString, out var percentValue) && double.TryParse(discountAmountString, out var discountAmount))
                {
                    percentValue /= 100;

                    var discountValue = tt * percentValue;

                    discountValue = Math.Max(discountValue, 0);

                    var total = (double)tt - discountValue;

                    ViewBag.FirstDiscountAmount = discountAmount;
                    ViewBag.DiscountAmount = discountValue;
                    ViewBag.FirstValue = valueString;
                    ViewBag.Value = percentValue;
                    ViewBag.VoucherCode = voucherCode;

                    ViewBag.Total = total;
                }
            }
            ViewBag.TT = tt;
            ViewBag.Total = ViewBag.TT;

            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            if (account != null)
            {
                var dc = getapiAddress.GetApi("Address").FirstOrDefault(c => c.AccountId == account.Id);
                if (dc != null)
                {
                    var p = await province(dc.Province);
                    if (p != 0)
                    {
                        var d = await dis(dc.District, p);
                        if (d != 0)
                        {
                            var w = await wad(dc.Ward, d);
                            int sship = await getServiceShip(d);

                            client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee?service_id={sship}" + $"&insurance_value=100000&to_ward_code={w}" + $"&to_district_id={d}" + "&from_district_id=3440" + $"&weight={can}", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");

                            // Gọi API để lấy danh sách các tỉnh/thành phố

                            var fee = await client.GetFeeshipAsync();

                            //Kiểm tra kết quả trả về
                            if (fee.Code == 200) // Thành công
                            {
                                // Trả về danh sách các quận/huyện dưới dạng JSON
                                ViewBag.fee = fee.Data.total;
                            }
                        }

                    }


                }
                else
                {
                    ViewBag.fee = 0;
                }

                return View(dc);
            }

            return View();
        }

        public async Task<IActionResult> Checkout()
        {
            try
            {
                var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
                if (User.Identity.IsAuthenticated)
                {

                    var Uid = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                    account = getapiAc.GetApi("Account").FirstOrDefault(c => c.Id.ToString() == Uid);
                    SessionService.SetObjToJson(HttpContext.Session, "Account", account);
                }

                var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");

                // Gọi API để lấy danh sách các tỉnh/thành phố
                var response = await client.GetProvincesAsync();
                if (response.Code == 200) // Thành công
                {
                    // Trả về danh sách các quận/huyện dưới dạng JSON
                    ViewBag.province = response.Data;
                }

                //// fee ship
                var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");
                var can = 100;
                if (products.Count != 0)
                {
                    var sl = 0;
                    foreach (var item in products)
                    {
                        sl += item.Quantity;
                    }
                    can = sl * 100;
                }

                ViewBag.Product = products;


                var tt = 0;
                // Lấy thông tin voucher từ TempData
                var discountAmountString = TempData["DiscountAmount"] as string;
                var voucherCode = TempData["VoucherCode"] as string;
                var valueString = TempData["Value"] as string;

                foreach (var item in ViewBag.Product)
                {

                    tt += (item.Quantity * item.Price);
                }
                if (!string.IsNullOrEmpty(voucherCode))
                {

                    if (double.TryParse(valueString, out var percentValue) && double.TryParse(discountAmountString, out var discountAmount))
                    {
                        percentValue /= 100;

                        var discountValue = tt * percentValue;

                        discountValue = Math.Max(discountValue, 0);

                        var total = (double)tt - discountValue;

                        ViewBag.FirstDiscountAmount = discountAmount;
                        ViewBag.DiscountAmount = discountValue;
                        ViewBag.FirstValue = valueString;
                        ViewBag.Value = percentValue;
                        ViewBag.VoucherCode = voucherCode;

                        ViewBag.Total = total;
                    }
                }
                ViewBag.TT = tt;
                ViewBag.Total = tt;


                if (account.Id != Guid.Empty)
                {

                    
                    var dc = getapiAddress.GetApi("Address").FirstOrDefault(c => c.AccountId == account.Id);

                    if (dc != null)
                    {
                        var p = await province(dc.Province);
                        if (p != 0)
                        {
                            var d = await dis(dc.District, p);
                            if (d != 0)
                            {
                                var w = await wad(dc.Ward, d);
                                int sship = await getServiceShip(d);

                                client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee?service_id={sship}" + $"&insurance_value=100000&to_ward_code={w}" + $"&to_district_id={d}" + "&from_district_id=3440" + $"&weight={can}", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");

                                // Gọi API để lấy danh sách các tỉnh/thành phố

                                var fee = await client.GetFeeshipAsync();

                                //Kiểm tra kết quả trả về
                                if (fee.Code == 200) // Thành công
                                {
                                    // Trả về danh sách các quận/huyện dưới dạng JSON
                                    ViewBag.fee = fee.Data.total;
                                }
                            }

                        }


                    }

                    else
                    {
                        ViewBag.fee = 0;
                    }

                    return View(dc);
                }
                if (ViewBag.fee == null)
                {
                    ViewBag.fee = 0;
                }
                return View();
            }
            catch (Exception )
            { 
                _notyf.Error("Lỗi");
                return View();
              }
           
          
        }

        [HttpPost]
        public async Task<IActionResult> SaveVoucherForUser(Guid voucherId)
        {
            try
            {
                var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");

                if (account == null || account.Id == Guid.Empty)
                {
                    return RedirectToAction("Login", "Account");
                }

                var voucher = getapiVoucher.GetApi("Voucher").FirstOrDefault(v => v.Id == voucherId);

                if (voucher != null)
                {
                    // Kiểm tra xem người dùng đã có phiếu giảm giá này chưa
                    var existingVoucherAcc = getapiVoucherAcc.GetApi("VoucherForAcc").FirstOrDefault(v => v.Id_Voucher == voucherId && v.Id_Account == account.Id);

                    if (existingVoucherAcc == null)
                    {
                        if (voucher.Quantity > 0)
                        {
                            // Tạo mới một đối tượng VoucherForAcc và lưu nó vào cơ sở dữ liệu
                            var voucherForAcc = new VoucherForAcc()
                            {
                                Id = Guid.NewGuid(),
                                Id_Account = account.Id,
                                Id_Voucher = voucher.Id,
                                Code = voucher.Code,
                                Name = voucher.Name,
                                Value = voucher.Value,
                                DiscountAmount = voucher.DiscountAmount,
                                EndDate = voucher.EndDate,
                                Status = voucher.Status,
                            };

                            await getapiVoucherAcc.CreateObj(voucherForAcc, "VoucherForAcc");

                            voucher.Quantity--;

                            await getapiVoucher.UpdateObj(voucher, "Voucher");
                            TempData["VoucherError"] = "Lưu thành công";
                        }
                        else
                        {
                            TempData["VoucherError"] = "Chúc bạn may mắn lần sau";
                        }
                    }
                    else
                    {
                        TempData["VoucherError"] = "Phiếu giảm giá đã có trong tài khoản của bạn.";                     
                        _notyf.Success("Voucher đã có trong tài khoản của bạn!");

                    }
                }
                else
                {

                    TempData["VoucherError"] = "Phiếu giảm giá không hợp lệ";

                   
                    _notyf.Warning("Chúc bạn may mắn lần sau!");

                }
            }
            catch (Exception ex)
            {


                _logger.LogError(ex, "Đã xảy ra lỗi khi lưu phiếu giảm giá");
                TempData["VoucherError"] = "Đã xảy ra lỗi khi lưu phiếu giảm giá";
            }
            return RedirectToAction("Details", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ApplyDiscount()
        {

            try
            {
                // Lấy thông tin tài khoản từ session
                var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");

                if (account == null || account.Id == Guid.Empty)
                {
                    // Xử lý trường hợp người dùng chưa đăng nhập
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    // Lấy danh sách voucher cho tài khoản
                    var voucherAcc = getapiVoucherAcc.GetApi("VoucherForAcc").Where(c => c.Id_Account == account.Id && c.Status == 1).ToList();

                    if (voucherAcc != null && voucherAcc.Any())
                    {
                        return View(voucherAcc);
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý nếu có lỗi xảy ra
                return StatusCode(500, new { ErrorMessage = $"Lỗi viewáy chủ nội bộ: {ex.Message}" });
            }

        }
        [HttpPost]
        public async Task<IActionResult> ApplyDiscount(string selectedVoucher)
        {
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");

            if (account == null || account.Id == Guid.Empty)
            {
                // xử lý trường hợp người dùng chưa đăng nhập
                return RedirectToAction("Login", "Account");
            }
            var voucher = getapiVoucherAcc.GetApi("VoucherForAcc").FirstOrDefault(d => d.Code == selectedVoucher && d.EndDate >= DateTime.Now);
            if (voucher != null)
            {
                TempData["DiscountAmount"] = voucher.DiscountAmount.ToString();
                TempData["Value"] = voucher.Value.ToString();
                TempData["VoucherCode"] = voucher.Code;

                return RedirectToAction("Checkout");
            }
            else
            {
                // xử lý khi mã giảm giá không hợp lệ
                ModelState.AddModelError("Error", "Mã giảm giá không hợp lệ");
                return RedirectToAction("Checkout");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ApplyDiscount2()
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
                Value = v.Value,
                DiscountAmount = v.DiscountAmount,
                EndDate = v.EndDate,
            }).ToList();
            return View(view);

        }
        [HttpPost]
        public async Task<IActionResult> ApplyDiscount2(string selectedVoucher)
        {
            // Lấy thông tin voucher và tính số ngày còn lại
            var voucher = getapiVoucher.GetApi("Voucher").FirstOrDefault(d => d.Code == selectedVoucher && d.EndDate >= DateTime.Now);
            if (voucher != null)
            {
                TempData["DiscountAmount"] = voucher.DiscountAmount.ToString();
                TempData["Value"] = voucher.Value.ToString();
                TempData["VoucherCode"] = voucher.Code;

                return RedirectToAction("CheckOutOnl");
            }
            else
            {
                // Xử lý khi mã giảm giá không hợp lệ
                _notyf.Error("Mã giảm giá không hợp lệ");
                return RedirectToAction("CheckOutOnl");
            }
        }



        

        public async Task<IActionResult> Payment(Bill bill)

        {
            string url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string returnUrl = $"https://localhost:7095/Home/PaymentConfirm?id={bill.id}";
            string tmnCode = "6AV1KO3E";
            string hashSecret = "UGHKKYGUTTLWWTQOJBECDFAMDHZDBLWW";

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", ((long)(bill.TotalMoney * 100)).ToString());
            // pay.AddRequestData("vnp_Amount", "1000000"); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn");//Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);


            return Redirect(paymentUrl);

        }

        public async Task<IActionResult> PaymentConfirm(Guid id)
        {
            if (Request.QueryString.Value != null)
            {

                string hashSecret = "UGHKKYGUTTLWWTQOJBECDFAMDHZDBLWW"; //Chuỗi bí mật
                var vnpayData = Request.Query;
                PayLib pay = new PayLib();
                var Bill = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);
                var account = getapiAc.GetApi("Account").FirstOrDefault(c => c.Id == Bill.AccountId).Name;

                //lấy toàn bộ dữ liệu được trả về
                foreach (var (key, value) in vnpayData)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(key, value);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.Query["vnp_SecureHash"]; //hash của dữ liệu trả về
                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;


                       
                        if (Bill != null)
                        {

                            ///phương thức thanh toán
                            var PM = getapiPM.GetApi("PaymentMethod").FirstOrDefault(c => c.Method == "Online");
                            if (PM == null)
                            {
                                PM = new PaymentMethod();
                                PM.id = Guid.NewGuid();
                                PM.Method = "Online";
                                PM.Status = 1;
                                PM.CreateDate = DateTime.Now;
                                PM.UpdateDate = DateTime.Now;
                                PM.Description = "Thanh toán online";

                                await getapiPM.CreateObj(PM, "PaymentMethod");
                            }
                            var pmd = new PaymentMethodDetail()
                            {
                                id = Guid.NewGuid(),
                                BillId = id,
                                PaymentMethodID = PM.id,
                                Status = 1,
                                TotalMoney = Bill.TotalMoney.ToString(),
                                Description = vnpayTranId.ToString(),
                            };
                            await getapiPMD.CreateObj(pmd, "PaymentMethodDetail");
                            Bill.Type = "Online - Đã Thanh Toán";
                            Bill.Code = orderId.ToString();
                            await bills.UpdateObj(Bill, "Bill");
                            
                        }
                        var products = SessionService.GetObjFromSession(HttpContext.Session, "Cart");


                        foreach (var item in products)
                        {
                            var productcartdetails = getapiCD.GetApi("CartDetails").FirstOrDefault(c => c.ProductDetail_ID == item.Id);

                            var p = products.Find(c => c.Id == item.Id);



                            if (productcartdetails != null)
                            {
                                await getapiCD.DeleteObj(productcartdetails.id, "CartDetails");

                            }
                        }
                        products.Clear();
                        SessionService.SetObjToJson(HttpContext.Session, "Cart", products);
                        _notyf.Success("Đặt hàng thành công");
                        if (account== "khach k dang nhap")
                        {
                            return RedirectToAction("Index");
                        }
                        
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                        _notyf.Error("Đặt hàng thất bại");
                        if (account == "khach k dang nhap")
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    _notyf.Error("Đặt hàng thất bại");
                    if (account == "khach k dang nhap")
                    {
                        return RedirectToAction("Index");
                    }
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                     
                }
            }
            return RedirectToAction("thongtin");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        }
    }
}