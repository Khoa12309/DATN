using _APPAPI.Service;
using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Data;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QLBillsConTroller : Controller
    {

        // GET: QLBills
        private readonly ILogger<QLBillsConTroller> _logger;
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
        private Getapi<Account> _account;
        public QLBillsConTroller(ILogger<QLBillsConTroller> logger)
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
            _account = new Getapi<Account>();
        }

        public ActionResult Index()
        {
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

                    return BadRequest("Mặt hàng này trong kho không đủ ");
                }
                else
                {
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
            return RedirectToAction("ShowBillXacNhan");
        }
        public async Task<IActionResult> HuyDon(Guid id)
        {
            var x = bills.GetApi("Bill").FirstOrDefault(c => c.id == id);
            var y = billDetails.GetApi("BillDetail").Where(c => c.BIllId == id).ToList();
            foreach (var item in y)
            {
                await billDetails.DeleteObj(item.id, "BillDetail");
            }
            await bills.DeleteObj(id, "Bill");
            return RedirectToAction("DonHuy");
        }

        public ActionResult ShowBill(string search)
        {
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var userBills = bills.GetApi("Bill").Where(c => c.Status == 1).OrderByDescending(d => d.CreateDate).ToList();
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
                    var tk = bills.GetApi("Bill").Where(c => c.Status == 1 && c.Code.Contains(search)).OrderByDescending(d => d.CreateDate).ToList();
                    ViewBag.viewbill = tk;
                    return View(tk);
                }
                else
                {
                    ViewBag.viewbill = userBills;




                    return View(userBills);
                }

            }
            catch (Exception ex)
            {
                return View(userBills);
            }
            return View(userBills);

        }

        public ActionResult DonHuy(string search)
        {
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var userBills = bills.GetApi("Bill").Where(c => c.Status == 0).OrderByDescending(d => d.CreateDate).ToList();
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
                    var tk = bills.GetApi("Bill").Where(c => c.Status == 0 && c.Code.Contains(search)).OrderByDescending(d => d.CreateDate).ToList();
                    ViewBag.viewbill = tk;
                    return View(tk);
                }
                else
                {
                    ViewBag.viewbill = userBills;




                    return View(userBills);
                }

            }
            catch (Exception ex)
            {
                return View(userBills);
            }
            return View(userBills);

        }
        public ActionResult ShowBillXacNhan(string search)
        {

            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var userBills = bills.GetApi("Bill").Where(c => c.Status == 2).OrderByDescending(d => d.CreateDate).ToList();
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
                    var tk = bills.GetApi("Bill").Where(c => c.Status == 2 && c.Code.Contains(search)).OrderByDescending(d => d.CreateDate).ToList();
                    ViewBag.viewbill = tk;
                    return View(tk);
                }
                else
                {
                    ViewBag.viewbill = userBills;




                    return View(userBills);
                }
            }
            catch (Exception ex)
            {


                return View(userBills);

            }

            return View(userBills);
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
