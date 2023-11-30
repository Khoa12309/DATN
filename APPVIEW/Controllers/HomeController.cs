
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
        private Getapi<CartDetail> getapiCD;
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
            getapiAddress = new Getapi<Address>();
            getapiCD=new Getapi<CartDetail>();
        }

        public IActionResult Index()
        {
            var productDetails = getapi.GetApi("ProductDetails");
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

        public IActionResult Nhanhang(Guid id)
        {
            var x = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);
            x.Status = 4;
            bills.UpdateObj(x, "Bill");
            return RedirectToAction("Thongtin");
        }  public IActionResult HuyDon( Guid id)
        {
            var x = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);
            x.Status = 0;
            bills.UpdateObj(x, "Bill");
            return RedirectToAction("Thongtin");
        }
        
        // Sử dụng:
        // Tạo chuỗi có độ dài 8 ký tự


        public async Task<IActionResult> DatHangN(Address obj,string pay,float phiship )
        {
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");

            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetProvincesAsync();
            foreach (var item in response.Data)
            {
                if (item.ProvinceID.ToString() ==obj.Province)
                {
                   obj.Province = item.ProvinceName; break;
                }
            }
            var bill = new Bill();
            bill.id = Guid.NewGuid();
            bill.AccountId = account.Id;
            bill.Code = GenerateRandomString(8);
            bill.PhoneNumber = obj.PhoneNumber;
            bill.Address = obj.Province+"-"+obj.District+"-"+obj.Ward+"-"+obj.SpecificAddress; 
            bill.CreateBy = DateTime.Now;
            bill.CreateDate = DateTime.Now;
            bill.UpdateBy = DateTime.Now;
            bill.ShipFee = phiship;
            bill.TotalMoney = phiship;
            bill.Status = 1;

            if (pay == "Online")
            {
                bill.Type = "Online";
            }
            else
            {
                bill.Type = "shipCod";
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


            if (pay == "Online")
            {

                return Payment(bill);
            }
            else
            {
                return RedirectToAction("Thongtin");
            }    
            
        }


        public async Task< IActionResult>DatHang(Guid size, Guid color, Guid productId, int soluong, string sdt, float ship ,int province , string district , string ward , string diachict)

        {

            string province2 = "";
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");


            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
         
            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetProvincesAsync();
            foreach (var item in response.Data)
            {
                if (item.ProvinceID == province ) { 
                
                   province2 = item.ProvinceName; break;
                }
            }

            // Gọi API để lấy danh sách các tỉnh/thành phố

            var diachi =diachict+"-"+ ward+"-"+district + "-Tỉnh "+province2 ;
         


            var x = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id_Product == productId && c.Id_Size == size && c.Id_Color == color);

            if (account.Id == Guid.Empty)
            {
                return Redirect("~/Account/Login");
            }
            else if (x == null )
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
                bill.ShipFee = ship;

                await bills.CreateObj(bill, "Bill");

                var billct = new BillDetail();


                billct.ProductDetailID = x.Id;
                billct.BIllId = bill.id;
                billct.Amount = soluong;
                billct.Price = soluong * x.Price;
                billct.Status = 1;
                await billDetails.CreateObj(billct, "BillDetail");
                bill.TotalMoney = billct.Price + ship;
                await bills.UpdateObj(bill, "Bill");
                ViewBag.Bill = bill;
                ViewBag.Billct = billct;
                ViewBag.ctsp = x; 
                ViewBag.sp = getapiProduct.GetApi("Product").FirstOrDefault(c => c.Id == x.Id_Product);
                ViewBag.sizee = getapiSize.GetApi("Size").FirstOrDefault(c => c.Id == x.Id_Size);
                ViewBag.Collor = getapiColor.GetApi("Color").FirstOrDefault(c => c.Id == x.Id_Color);

            }



            return View();
        }
        public IActionResult Thongtin()
        {
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var userBills = bills.GetApi("Bill").Where(c => c.AccountId == account.Id&& c.Status!=4 ).OrderByDescending(d => d.CreateDate).ToList();
            ViewBag.viewbill = userBills;

            if (account.Id == Guid.Empty)
            {
                return BadRequest("Bạn chưa đăng nhập");
            }

            var billDetailsApi = billDetails.GetApi("BillDetail");
            var productDetailsApi = getapi.GetApi("ProductDetails");
            var productsApi = getapiProduct.GetApi("Product");

            ViewBag.viewbillct = billDetailsApi;
            ViewBag.viewprdct = productDetailsApi;
            ViewBag.viewprd = productsApi;
            ViewBag.sizee = getapiSize.GetApi("Size");

            ViewBag.Collor = getapiColor.GetApi("Color");
            return View(userBills);

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
            
            
            
            ViewBag.Category =  getapiCategory.GetApi("Category");
            ViewBag.Supplier = getapiSupplier.GetApi("Supplier");         
            ViewBag.Material =  getapiMaterial.GetApi("Material");

            return View(pro);
        }

        public async Task<IActionResult> province()
        {
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetProvincesAsync();           
            //Kiểm tra kết quả trả về
            if (response.Code == 200) // Thành công
            {
                // Trả về danh sách các quận/huyện dưới dạng JSON

                ViewBag.province = response.Data;

                return View();
            }
            else // Thất bại
            {
                // Trả về thông báo lỗi
                return BadRequest(response.Message);
            }


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
            var can=100;
            if (products.Count!=0)
            {
                var sl = 0;
                foreach (var item in products)
                {
                    sl += item.Quantity;
                }
              can = sl*100;
            }           
            int sship = await getServiceShip(data.to_district_id);
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee?service_id={sship}" + $"&insurance_value=100000&to_ward_code={data.towardcode.ToString()}" + $"&to_district_id={data.to_district_id.ToString()}" + "&from_district_id=3440"+$"&weight={can}", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
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
        public async Task<IActionResult> Checkout()
        {
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
            // Lấy thông tin voucher từ TempData
            var discountAmountString = TempData["DiscountAmount"] as string;
            var voucherCode = TempData["VoucherCode"] as string;
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
            if (double.TryParse(discountAmountString, out var discountAmount))
            {
                ViewBag.DiscountAmount = discountAmount;
                ViewBag.TT -= discountAmount;
                ViewBag.VoucherCode = voucherCode;

            }
            ViewBag.TT = tt;
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            if (account!= null)
            {
                var dc = getapiAddress.GetApi("Address").FirstOrDefault(c => c.AccountId == account.Id);
                return View(dc);
            }
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

    
        public ActionResult Payment(Bill bill)
        {
            string url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string returnUrl = "https://localhost:7095/Home/PaymentConfirm";
            string tmnCode = "OQK7ZU4V";
            string hashSecret = "WRKKYLZIEYLLPPFRNNQXVAKXHKGRIEEA";
            
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

        public ActionResult PaymentConfirm()
        {
            if(Request.QueryString.Value !=null)
            {
               
                string hashSecret = "WRKKYLZIEYLLPPFRNNQXVAKXHKGRIEEA"; //Chuỗi bí mật
                var vnpayData = Request.Query;
                PayLib pay = new PayLib();


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
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}