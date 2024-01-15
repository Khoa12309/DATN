using _APPAPI.Service;
using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;


using System.IO;
using System.Data;
using Image = APPDATA.Models.Image;

using System.Drawing;
using System.Net.NetworkInformation;
using Microsoft.CodeAnalysis;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = iTextSharp.text.Font;
using Size = APPDATA.Models.Size;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;

using APPVIEW.ViewModels;


using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Drawing.Charts;
using X.PagedList;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class QLBillsConTroller : Controller
    {

        // GET: QLBills
        private readonly ILogger<QLBillsConTroller> _logger;
        private Getapi<ProductDetail> getapi;
        private Getapi<Category> getapiCategory;
        private Getapi<APPDATA.Models.Color> getapiColor;
        private Getapi<Image> getapiImg;
        private Getapi<Size> getapiSize;
        private Getapi<Supplier> getapiSupplier;
        private Getapi<Product> getapiProduct;
        private Getapi<Material> getapiMaterial;
        private Getapi<Bill> bills;
        private Getapi<BillDetail> billDetails;
        private Getapi<Account> _account;
        public INotyfService _notyf;
        private static readonly Random random = new Random();
        private string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public QLBillsConTroller(ILogger<QLBillsConTroller> logger, INotyfService notyf)
        {
            _logger = logger;
            getapi = new Getapi<ProductDetail>();
            getapiCategory = new Getapi<Category>();
            getapiColor = new Getapi<APPDATA.Models.Color>();
            getapiImg = new Getapi<Image>();
            getapiSize = new Getapi<Size>();
            getapiSupplier = new Getapi<Supplier>();
            getapiProduct = new Getapi<Product>();
            getapiMaterial = new Getapi<Material>();
            bills = new Getapi<Bill>();
            billDetails = new Getapi<BillDetail>();
            _account = new Getapi<Account>();
            _notyf = notyf;

        }

        public ActionResult Index()
        {
            return View();
        }
        public IActionResult Chitiet(Guid id ) {
            var bill = bills.GetApi("Bill").FirstOrDefault(c=>c.id == id);
            if (bill != null) {
                ViewBag.bil = bill;
                var billct = billDetails.GetApi("BillDetail").Where(c=>c.BIllId==bill.id);
                ViewBag.prd = getapi.GetApi("ProductDetails");
                ViewBag.size = getapiSize.GetApi("Size");
                ViewBag.color = getapiColor.GetApi("Color");
                return View(billct);
            }



            return View();
        }


        public async Task<IActionResult> Xacnhan(Guid id)
        {
            var x = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);

            var billct = billDetails.GetApi("BillDetail").Where(c => c.BIllId == id);
            foreach (var item in billct)
            {
                var prdct = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id == item.ProductDetailID);
                var sl = prdct.Quantity = prdct.Quantity - item.Amount;

                if (sl < 0)
                {
                    _notyf.Warning("Mặt hàng này trong kho không đủ");
                    return RedirectToAction("ShowBill");
                }
                else
                {
                    if (sl==0)
                    {
                        prdct.Status = 0;
                    }
                    _notyf.Success("Đã xác nhận đơn hàng!");
                    prdct.Quantity = sl;
                    await getapi.UpdateObj(prdct, "ProductDetails");
                    x.Status = 2;
                    await bills.UpdateObj(x, "Bill");

                }
            }

            return RedirectToAction("ShowBill");
        }

        public async Task<IActionResult> GiaoHang(Guid id)
        {
            var x = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);
            x.Status = 3;
            await bills.UpdateObj(x, "Bill");
            _notyf.Success("Đã xác nhận giao hàng");
            return RedirectToAction("ShowBillXacNhan");
        }
        public async Task<IActionResult> HuyDon(Guid id)
        {

            var x = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);
            var y = billDetails.GetApi("BillDetail").Where(c => c.BIllId == id).ToList();
            if (x.Status == 5)
            {

                foreach (var item in y)
                {  
                    var pr = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id == item.ProductDetailID);
                    pr.Quantity += item.Amount;
                    await getapi.UpdateObj(pr, "ProductDetails");
                 
                }
            }
            x.Status = 10;
            await bills.UpdateObj(x, "Bill");
            _notyf.Success("Đã xác nhận hủy đơn");
            return RedirectToAction(nameof(ShowBill));
        }
        public async Task<IActionResult> HuyDon2(Guid id)
        {

            var x = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);
            var y = billDetails.GetApi("BillDetail").Where(c => c.BIllId == id).ToList();
            if (x.Status == 5)
            {

                foreach (var item in y)
                {
                    var pr = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id == item.ProductDetailID);
                    pr.Quantity += item.Amount;
                    await getapi.UpdateObj(pr, "ProductDetails");
                 
                }
            }
            x.Status = 10;
            await bills.UpdateObj(x, "Bill");
            _notyf.Success("Đã xác nhận hủy đơn");
            return RedirectToAction("DonHuy");
        }

        public ActionResult ShowBill(string search,int?page)
        {
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var userBills = bills.GetApi("Bill").Where(c => c.Status == 1).OrderByDescending(d => d.CreateDate).ToPagedList(pageNumber, pageSize);
            ViewBag.viewbill = userBills;

            var billDetailsApi = billDetails.GetApi("BillDetail");
            var productDetailsApi = getapi.GetApi("ProductDetails");
            var productsApi = getapiProduct.GetApi("Product");


            ViewBag.viewbillct = billDetailsApi;
            ViewBag.viewprdct = productDetailsApi;
            ViewBag.viewprd = productsApi;
            ViewBag.sizee = getapiSize.GetApi("Size");
            ViewBag.acc = _account.GetApi("Account");
            ViewBag.Collor = getapiColor.GetApi("Color");
            try
            {
                if (search != null||search!="")
                {

                    var tk = userBills.Where(c => c.Status == 1 && c.Code.ToLower().Contains(search.ToLower()) || c.Name.ToLower().Contains(search.ToLower())).OrderByDescending(d => d.CreateDate).ToList();
                    var pagedList = tk.ToPagedList(pageNumber, pageSize);
                    ViewBag.viewbill = pagedList;
                    return View(pagedList);
                }
                else
                {
                    return View(userBills);
                }

            }
            catch (Exception ex)
            {
                return View(userBills);
            }

        }
        public ActionResult ShowDonCho(string search, int? page)
        {
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var userBills = bills.GetApi("Bill").Where(c => c.Status == 9).OrderByDescending(d => d.CreateDate).ToPagedList(pageNumber, pageSize);
            ViewBag.viewbill = userBills;

            var billDetailsApi = billDetails.GetApi("BillDetail");
            var productDetailsApi = getapi.GetApi("ProductDetails");
            var productsApi = getapiProduct.GetApi("Product");


            ViewBag.viewbillct = billDetailsApi;
            ViewBag.viewprdct = productDetailsApi;
            ViewBag.viewprd = productsApi;
            ViewBag.sizee = getapiSize.GetApi("Size");
            ViewBag.acc = _account.GetApi("Account");
            ViewBag.Collor = getapiColor.GetApi("Color");
            try
            {
                if (search != null || search != "")
                {

                    var tk = userBills.Where(c => c.Status == 1 && c.Code.ToLower().Contains(search.ToLower()) || c.Name.ToLower().Contains(search.ToLower())).OrderByDescending(d => d.CreateDate).ToList();
                    var pagedList = tk.ToPagedList(pageNumber, pageSize);
                    ViewBag.viewbill = pagedList;
                    return View(pagedList);
                }
                else
                {
                    return View(userBills);
                }

            }
            catch (Exception ex)
            {
                return View(userBills);
            }

        }
        public ActionResult DonHuy(string search,int?page)
        {
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (User.Identity.IsAuthenticated)
            {

                var Uid = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                var acc = _account.GetApi("Account").FirstOrDefault(c => c.Id.ToString() == Uid);
                SessionService.SetObjToJson(HttpContext.Session, "Account", acc);
            }
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var userBills = bills.GetApi("Bill").Where(c => c.Status == 0).OrderByDescending(d => d.CreateDate).ToPagedList(pageNumber, pageSize);
            ViewBag.viewbill = userBills;


            var billDetailsApi = billDetails.GetApi("BillDetail");
            var productDetailsApi = getapi.GetApi("ProductDetails");
            var productsApi = getapiProduct.GetApi("Product");
            ViewBag.viewbillct = billDetailsApi;
            ViewBag.viewprdct = productDetailsApi;
            ViewBag.viewprd = productsApi;
            ViewBag.sizee = getapiSize.GetApi("Size");
            ViewBag.acc = _account.GetApi("Account");
            ViewBag.Collor = getapiColor.GetApi("Color");
            try
            {
                if (search != "")
                {
                    var tk = userBills.Where(c => c.Status == 2 && c.Code.ToLower().Contains(search.ToLower()) || c.Name.ToLower().Contains(search.ToLower())).OrderByDescending(d => d.CreateDate).ToList();
                    var pagedList = tk.ToPagedList(pageNumber, pageSize);
                    ViewBag.viewbill = pagedList;
                    return View(pagedList);
                }
                else
                {
                    _notyf.Information("Không tìm thấy đơn hàng!");
                    return View(userBills);
                }

            }
            catch (Exception ex)
            {
                return View(userBills);
            }


        }
        public ActionResult ShowBillXacNhan(string search,int? page)
        {
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var userBills = bills.GetApi("Bill").Where(c => c.Status == 2).OrderByDescending(d => d.CreateDate).ToPagedList(pageNumber, pageSize);
            var billDetailsApi = billDetails.GetApi("BillDetail");
            var productDetailsApi = getapi.GetApi("ProductDetails");
            var productsApi = getapiProduct.GetApi("Product");
            ViewBag.viewbillct = billDetailsApi;
            ViewBag.viewprdct = productDetailsApi;
            ViewBag.viewprd = productsApi;
            ViewBag.sizee = getapiSize.GetApi("Size");
            ViewBag.acc = _account.GetApi("Account");
            ViewBag.Collor = getapiColor.GetApi("Color");
            ViewBag.viewbill = userBills;
            try
            {

                if (search != "")
                {
                    var tk = userBills.Where(c => c.Status == 2 && c.Code.ToLower().Contains(search.ToLower()) || c.Name.ToLower().Contains(search.ToLower())).OrderByDescending(d => d.CreateDate).ToList();
                    var pagedList = tk.ToPagedList(pageNumber, pageSize);
                    ViewBag.viewbill = pagedList;
                    return View(pagedList);
                }
                else
                {
                  
                    return View(userBills);
                }
            }
            catch (Exception ex)
            {
                return View(userBills);
            }
        }
        [HttpPost]
        public async Task<ActionResult> ChosenProduct(Guid productId)
        {
            var product = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id == productId);
            List<ProductDetail> products = new List<ProductDetail>();

            if (product != null)
            {
                // Kiểm tra xem sản phẩm đã có trong danh sách chưa
                bool productExists = products.Any(p => p.Id == productId);

                if (!productExists)
                {
                    products.Add(product);
                }
                var size = getapiSize.GetApi("Size").FirstOrDefault(c => c.Id == product.Id_Size);
                var color = getapiColor.GetApi("Color").FirstOrDefault(c => c.Id == product.Id_Color);
                return Json(new { success = true, sanphamchitiets = products, size = size.Name, color = color.Name });
            }

            return Json(new { success = false });
        }
        public ActionResult TimKiem(string searchText)
        {
            var size = getapiSize.GetApi("Size");
            var color = getapiColor.GetApi("Color");
            var Img = getapiImg.GetApi("Image");
            var prd = getapi.GetApi("ProductDetails").Where(c => c.Quantity > 0 && c.Status != 0);
            if (searchText != null)
            {
                var products = getapi.GetApi("ProductDetails").Where(c => c.Quantity > 0 && c.Status != 0 && c.Name.ToLower().Contains(searchText.ToLower().Trim())).ToList();
                return Json(new { success = true, productct = products, size = size, color = color,img = Img });
            }
            return Json(new { success = true, productct = prd, size = size, color = color, img = Img });
        }


        public async Task<ActionResult> BanHangOff(string inputValue, Guid id)
        {
            ViewBag.bil = bills.GetApi("Bill");
            if (id!=Guid.Empty)
            {

                var billdt = billDetails.GetApi("BillDetail").Where(c => c.BIllId == id);
                if (billdt.Count() > 0)
                {
                    ViewBag.size = getapiSize.GetApi("Size");
                    ViewBag.color = getapiColor.GetApi("Color");
                    ViewBag.Img = getapiImg.GetApi("Image");


                    ViewBag.productdt = getapi.GetApi("ProductDetails").Where(c => c.Quantity > 0 && c.Status != 0).ToList();
                    return View(billdt);
                }

            }
         else {


                ViewBag.size = getapiSize.GetApi("Size");
                ViewBag.color = getapiColor.GetApi("Color");
                ViewBag.Img = getapiImg.GetApi("Image");


                ViewBag.productdt = getapi.GetApi("ProductDetails").Where(c => c.Quantity > 0 && c.Status != 0).ToList();
                return View();
            }




            return View();
        }


        public string GenerateRandomString(int length)
        {
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        [HttpPost]
        public async Task<IActionResult> CreateBill(List<Guid> productId, List<int> soluong, float tongtien, string tenkh,string sdt, Guid idbill)
        {
            var prdct = getapi.GetApi("ProductDetails").ToList();
            var billct = billDetails.GetApi("BillDetail");
            var bill = bills.GetApi("Bill");
            if (!User.Identity.IsAuthenticated)
            {

                var Uid = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                var acc = _account.GetApi("Account").FirstOrDefault(c => c.Id.ToString() == Uid);
                SessionService.SetObjToJson(HttpContext.Session, "Account", acc);
                return Redirect("~/Account/Login");
            }
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            if (tenkh == "" || tenkh == null)
            {


                tenkh = "Không Lưu Tên";
            }
            if (productId.Count==0)
            {
                _notyf.Warning("Không có sản phẩm");
                return RedirectToAction("BanHangOff");
            }
            if (idbill != null && idbill != Guid.Empty) {

                var bctt = billct.Where(c => c.BIllId == idbill);
               var  hoaDonList = bctt.Where(hd => !productId.Contains(hd.ProductDetailID)).ToList();
                foreach (var item in hoaDonList)
                {
                  var sanpahm = prdct.FirstOrDefault(c=>c.Id == item.ProductDetailID);
                  sanpahm.Quantity += item.Amount;
                    await getapi.UpdateObj(sanpahm, "ProductDetails");
                    await  billDetails.DeleteObj(item.id,"BillDetail");
                }

                if (productId.Count == soluong.Count)
                {
                    for (int i = 0; i < productId.Count; i++)
                    {
                        var id = productId[i];
                        var quantity = soluong[i];
                       var bct=  billct.FirstOrDefault(c=>c.ProductDetailID == id && c.BIllId == idbill);

                        if (bct == null) {
                            foreach (var item in prdct)
                            {
                                if (item.Id == id)
                                {
                                    var bil = new BillDetail();
                                    bil.id = Guid.NewGuid();
                                    bil.BIllId = idbill;
                                    bil.Amount = quantity;
                                    bil.Price = quantity * item.Price;
                                    bil.Status = 1;
                                    bil.ProductDetailID = item.Id;
                                    await billDetails.CreateObj(bil, "BillDetail");

                                    item.Quantity = item.Quantity - quantity;
                                    if (item.Quantity == 0)
                                    {
                                        item.Status = 0;
                                    }
                                    await getapi.UpdateObj(item, "ProductDetails");


                                }
                            }

                        }
                        else {
                         var prrd = prdct.FirstOrDefault(c => c.Id == id);
                         var slcu = bct.Amount;
                         bct.Amount = quantity;
                         bct.Price = quantity*prrd.Price;
                         await billDetails.UpdateObj(bct,"BillDetail");
                         var slupdate = quantity - slcu;
                         prrd.Quantity  = prrd.Quantity+slupdate;
                         await getapi.UpdateObj(prrd, "ProductDetails");
                        }
                    }
                    var billl = bill.FirstOrDefault(c=>c.id==idbill);
                    billl.Status = 4;
                    billl.TotalMoney = tongtien;
                    billl.Name = tenkh;
                    billl.PhoneNumber = sdt;
                    billl.PayDate = DateTime.Now;
                    await bills.UpdateObj(billl,"Bill");
                    return RedirectToAction("GenerateInvoice", new { billId = idbill, tenkh = billl.Name });
                }

            }

            Bill newbil = new Bill();
            newbil.id = Guid.NewGuid();
            newbil.PhoneNumber = sdt;
            newbil.AccountId = account.Id;
            newbil.Code = GenerateRandomString(8);
            newbil.CreateDate = DateTime.Now;
            newbil.Type = "Tại Quầy";
            newbil.TotalMoney = tongtien;
            newbil.Status = 4;
            newbil.Name = tenkh;
            newbil.PayDate = DateTime.Now;  
            await bills.CreateObj(newbil, "Bill");
            if (productId.Count == soluong.Count)
            {
                for (int i = 0; i < productId.Count; i++)
                {
                    var id = productId[i];
                    var quantity = soluong[i];

                    foreach (var item in prdct)
                    {
                        if (item.Id == id)
                        {
                            var bil = new BillDetail();
                            bil.id = Guid.NewGuid();
                            bil.BIllId = newbil.id;
                            bil.Amount = quantity;
                            bil.Price = quantity * item.Price;
                            bil.Status = 1;
                            bil.ProductDetailID = item.Id;
                            await billDetails.CreateObj(bil, "BillDetail");

                            item.Quantity = item.Quantity - quantity;
                            if (item.Quantity==0)
                            {
                                item.Status = 0;
                            }
                            await getapi.UpdateObj(item, "ProductDetails");

                            
                        }
                    }
                }
          
                return RedirectToAction("GenerateInvoice", new { billId = newbil.id, tenkh = newbil.Name });
            }
            else
            {
                return RedirectToAction("BanHangOff");
            }
            
        }

        public async Task<IActionResult> ThanhToanTiep(Guid id)
        {
            var invoice = bills.GetApi("Bill").FirstOrDefault(c=>c.id == id); // Hàm lấy đối tượng Invoice từ database hoặc nơi lưu trữ khác
            

            return RedirectToAction("BanHangOff", new { billId = invoice.id });
        }

      
        public async Task<IActionResult> DonCho(List<Guid> productId, List<int> soluong, float tongtien, string tenkh, string sdt)
        {
            if (!User.Identity.IsAuthenticated)
            {

                var Uid = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                var acc = _account.GetApi("Account").FirstOrDefault(c => c.Id.ToString() == Uid);
                SessionService.SetObjToJson(HttpContext.Session, "Account", acc);
                return Redirect("~/Account/Login");
            }
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            if (tenkh == "" || tenkh == null)
            {


                tenkh = "Không Lưu Tên";
            }
            if (productId.Count == 0)
            {
                _notyf.Warning("Không có sản phẩm");
                return RedirectToAction("BanHangOff");
            }

            var prdct = getapi.GetApi("ProductDetails").ToList();
            var billct = billDetails.GetApi("BillDetail");
            var bill = bills.GetApi("Bill");
            Bill newbil = new Bill();
            newbil.id = Guid.NewGuid();
            newbil.PhoneNumber = sdt;
            newbil.AccountId = account.Id;
            newbil.Code = GenerateRandomString(8);
            newbil.CreateDate = DateTime.Now;
            newbil.Type = "Tại Quầy";
            newbil.TotalMoney = tongtien;
            newbil.Status = 9;
            newbil.Name = tenkh;

            await bills.CreateObj(newbil, "Bill");
            if (productId.Count == soluong.Count)
            {
                for (int i = 0; i < productId.Count; i++)
                {
                    var id = productId[i];
                    var quantity = soluong[i];

                    foreach (var item in prdct)
                    {
                        if (item.Id == id)
                        {
                            var bil = new BillDetail();
                            bil.id = Guid.NewGuid();
                            bil.BIllId = newbil.id;
                            bil.Amount = quantity;
                            bil.Price = quantity * item.Price;
                            bil.Status = 1;
                            bil.ProductDetailID = item.Id;
                            await billDetails.CreateObj(bil, "BillDetail");

                            item.Quantity = item.Quantity - quantity;
                            if (item.Quantity == 0)
                            {
                                item.Status = 0;
                            }
                            await getapi.UpdateObj(item, "ProductDetails");
                        }
                    }
                }
            }
            else
            {
                return RedirectToAction("BanHangOff");
            }
            _notyf.Success("Tạo đơn chờ thành công");
            return RedirectToAction("BanHangOff");
        }
        public string xulichuoi(string tenkh)
        {

            string normalizedString1 = tenkh.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString1)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            string normalizedString = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            // Chuyển chuỗi thành danh sách các từ
            string[] words = normalizedString.Split(' ');

            // Xử lý từng từ trong danh sách
            StringBuilder result = new StringBuilder();
            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    // Chuyển chữ cái đầu của từ thành chữ hoa
                    string processedWord = char.ToUpper(word[0]) + word.Substring(1).ToLower();

                    result.Append(processedWord);
                    result.Append(" "); // Thêm dấu cách sau mỗi từ
                }
            }

            var ten = result.ToString().Trim();

            return ten;
        }

        public async  Task<ActionResult> GenerateInvoice(Guid billId, string tenkh)
        {
            // Lấy thông tin hóa đơn từ billId

            var bill = bills.GetApi("Bill").FirstOrDefault(b => b.id == billId);
            var billDetailss = billDetails.GetApi("BillDetail").Where(bd => bd.BIllId == billId).ToList();
            var products = getapi.GetApi("ProductDetails").ToList();
            
            // Tạo file PDF


            using (var ms = new MemoryStream())
            {
                using (var document = new iTextSharp.text.Document(PageSize.A5, 25, 25, 30, 30))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    document.Open();

                    // Tạo tiêu đề hóa đơn
                    var titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
                    var titleParagraph = new Paragraph("Hoa Don Ban Hang Shop Super Fashion \n", titleFont);
                    titleParagraph.Alignment = Element.ALIGN_CENTER;
                    document.Add(titleParagraph);
                    var titleParagraph2 = new Paragraph("  ", titleFont);
                    titleParagraph2.Alignment = Element.ALIGN_CENTER;
                    document.Add(titleParagraph2);

                    // Tạo thông tin khách hàng và nhân viên
                    var infoFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL);
                    var infoTable = new PdfPTable(2);
                    infoTable.WidthPercentage = 100;
                    infoTable.SetWidths(new int[] { 1, 2 });

                    infoTable.AddCell(new Phrase("Ma Hoa Don :", infoFont));
                    infoTable.AddCell(new Phrase(bill.Code, infoFont));
                    infoTable.AddCell(new Phrase("Ngay Tao :", infoFont));
                    infoTable.AddCell(new Phrase(bill.CreateDate.ToString("dd/MM/yyyy"), infoFont));
                    infoTable.AddCell(new Phrase("Ten Khach Hang :", infoFont));
                    infoTable.AddCell(new Phrase(xulichuoi(tenkh).Replace("Đ", "D").Replace("đ", "d"), infoFont));

                    document.Add(infoTable);

                    // Tạo bảng chi tiết hóa đơn
                    var detailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL);
                    var detailTable = new PdfPTable(7);
                    detailTable.WidthPercentage = 100;
                    detailTable.SetWidths(new int[] { 1, 3, 1, 2, 2, 1, 2 });
                    detailTable.SpacingBefore = 10f;
                    detailTable.SpacingAfter = 10f;

                    detailTable.AddCell(new Phrase("STT", detailFont));
                    detailTable.AddCell(new Phrase("San Pham", detailFont));
                    detailTable.AddCell(new Phrase("Size", detailFont));
                    detailTable.AddCell(new Phrase("Mau", detailFont));
                    detailTable.AddCell(new Phrase("Don Gia", detailFont));
                    detailTable.AddCell(new Phrase("So Luong", detailFont));
                    detailTable.AddCell(new Phrase("Thanh Tien", detailFont));

                    int stt = 1;
                    foreach (var item in billDetailss)
                    {

                        var product = products.FirstOrDefault(p => p.Id == item.ProductDetailID);
                        var size = getapiSize.GetApi("Size").FirstOrDefault(c => c.Id == product.Id_Size);
                        var color = getapiColor.GetApi("Color").FirstOrDefault(c => c.Id == product.Id_Color);
                        detailTable.AddCell(new Phrase(stt.ToString(), detailFont));
                        detailTable.AddCell(new Phrase(product.Name, detailFont));
                        detailTable.AddCell(new Phrase(size.Name, detailFont));
                        detailTable.AddCell(new Phrase(xulichuoi(color.Name).Replace("Đ", "D").Replace("đ", "d"), detailFont));
                        detailTable.AddCell(new Phrase(product.Price.ToString("#,##0") + " VND", detailFont));
                        detailTable.AddCell(new Phrase(item.Amount.ToString(), detailFont));
                        detailTable.AddCell(new Phrase((item.Price).ToString("#,##0") + " VND", detailFont));
                        stt++;

                    }

                    document.Add(detailTable);

                    // Tạo tổng tiền và chữ ký
                    var totalFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                    var totalParagraph = new Paragraph("Tong Tien: " + bill.TotalMoney.ToString() + " VND");
                    totalParagraph.Alignment = Element.ALIGN_RIGHT;
                    document.Add(totalParagraph);

                    var signFont = FontFactory.GetFont("Arial", 12, Font.ITALIC);
                    var signParagraph = new Paragraph("\n\nNguoi Lap Hoa Don\n(Ky Va Ghi Ro Ho Ten)", signFont); signParagraph.Alignment = Element.ALIGN_RIGHT; document.Add(signParagraph);
                    document.Close();


                }

                // Trả về file PDF

                //_notyf.Success("Tạo hóa đơn thành công");

                return File(ms.ToArray(), "application/pdf", "HoaDon_" + bill.Code + ".pdf");
            }

        }
        public ActionResult ShowBillDaNhan(string search,int?page)
        {
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var userBills = bills.GetApi("Bill").Where(c => c.Status == 3).OrderByDescending(d => d.CreateDate).ToPagedList(pageNumber, pageSize);
            var billDetailsApi = billDetails.GetApi("BillDetail");
            var productDetailsApi = getapi.GetApi("ProductDetails");
            var productsApi = getapiProduct.GetApi("Product");


            ViewBag.viewbillct = billDetailsApi;
            ViewBag.viewprdct = productDetailsApi;
            ViewBag.viewprd = productsApi;
            ViewBag.sizee = getapiSize.GetApi("Size");
            ViewBag.acc = _account.GetApi("Account");
            ViewBag.Collor = getapiColor.GetApi("Color");
            ViewBag.viewbill = userBills;
            try
            {

                if (search != "")
                {
                    var tk = userBills.Where(c => c.Status == 3 && c.Code.ToLower().Contains(search.ToLower()) || c.Name.ToLower().Contains(search.ToLower())).OrderByDescending(d => d.CreateDate).ToList();
                    var pagedList = tk.ToPagedList(pageNumber, pageSize);
                    ViewBag.viewbill = pagedList;
                    return View(pagedList);
                }
                else
                {
                    return View(userBills);
                }
            }
            catch (Exception ex)
            {
                return View(userBills);

            }

            return View(userBills);
        }
        public async Task<IActionResult> Nhanhang(Guid id)
        {
            var x = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);
            x.Status = 4;
            x.PayDate = DateTime.Now;
            x.Type = "Đã nhận hàng và thanh toán";
            await bills.UpdateObj(x, "Bill");
            return RedirectToAction("ShowBillDaNhan");
        } 
        public async Task<IActionResult> KhongHuy(Guid id)
        {
            var x = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);
            if (x.Status == 0 ) {
               x.Status=1;

                await bills.UpdateObj(x, "Bill");
                return RedirectToAction("ShowBillDaNhan");
            }

            x.Status = 2;
            await bills.UpdateObj(x, "Bill");
            return RedirectToAction("ShowBillDaNhan");
        }
        [HttpPost]
        public ActionResult GetName(string sdt) { 



            var bill = bills.GetApi("Bill").FirstOrDefault(c => c.PhoneNumber ==sdt &&c.Name!=null&&c.Name!="" && c.Name != "Khong Luu Ten");
            if (bill != null && sdt!=null) {
                if (bill.Name != null || bill.Name != "")
                {
                    return Json(new { success = true, Name = bill.Name });
                }
              
            
            }




            return Json(new { success = true, Name = "" });


        }
        public async Task<IActionResult> Delete(Guid id)
        {
            var bd = billDetails.GetApi("BillDetail").Where(c => c.BIllId == id);
            var pd = getapi.GetApi("ProductDetails");
            foreach (var item in bd)
            {
                var x = pd.FirstOrDefault(c => c.Id == item.ProductDetailID);
                x.Quantity += item.Amount;
                await getapi.UpdateObj(x, "ProductDetails");
                await billDetails.DeleteObj(item.id,"BillDetail");
            }
           await bills.DeleteObj(id, "Bill");
            return RedirectToAction("ShowDonCho");

        }
        // POST: QLBills/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
