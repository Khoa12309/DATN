using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    public class BillController : Controller
    {
        private Getapi<Bill> getapi;
        private Getapi<Account> getapiacc;
        public BillController()
        {
            getapi = new Getapi<Bill>();
            getapiacc = new Getapi<Account>();
        }
  
        public async Task<IActionResult> GetList(string searchTerm)
        {
            var obj = getapi.GetApi("Bill").Where(c=>c.Type!= "Tại Quầy");
            ViewBag.account = getapiacc.GetApi("Account");
            try
            {
                if (searchTerm != "" || searchTerm!= null)
                {
                    var ac = getapiacc.GetApi("Account").Where(c=>c.Name.ToLower().Contains(searchTerm.ToLower())); ;
                    var tk = obj.Where(c=> c.Code.ToLower().Contains(searchTerm.ToLower()) || c.TotalMoney.ToString().Contains(searchTerm)).OrderByDescending(d => d.CreateDate).ToList();
                 
                    return View(tk);
                }
                else
                {





                    return View(obj);
                }

            }
            catch (Exception ex)
            {
                return View(obj);
            }
            return View(obj);

        }
        public async Task<IActionResult> GetList2(string searchTerm)
        {
            var obj = getapi.GetApi("Bill").Where(c => c.Type == "Tại Quầy");
            ViewBag.account = getapiacc.GetApi("Account");
            try
            {
                if (searchTerm != "" || searchTerm != null)
                {
                    var ac = getapiacc.GetApi("Account").Where(c => c.Name.ToLower().Contains(searchTerm.ToLower())); ;
                    var tk = obj.Where(c => c.Code.ToLower().Contains(searchTerm.ToLower()) || c.TotalMoney.ToString().Contains(searchTerm)).OrderByDescending(d => d.CreateDate).ToList();

                    return View(tk);
                }
                else
                {





                    return View(obj);
                }

            }
            catch (Exception ex)
            {
                return View(obj);
            }
            return View(obj);

        }
        public async Task<IActionResult> Search(string searchTerm)
        {
            var lstBill = getapi.GetApi("Bill");

            var searchResult = lstBill
                .Where(v =>
                    v.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Status.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            if (searchResult.Any())
            {
                return View("GetList", searchResult);
            }

            return NotFound("Voucher không tồn tại");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Bill obj)
        {
            try
            {
                getapi.CreateObj(obj, "Bill");
                
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }



        public async Task<IActionResult> Edit(Guid id)
        {

            var lst = getapi.GetApi("Bill");
            return View(lst.Find(c => c.id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Bill obj)
        {
            try
            {
                getapi.UpdateObj(obj, "Bill");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            getapi.DeleteObj(id, "Bill");
            return RedirectToAction("GetList");

        }
    }
}
