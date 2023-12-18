using _APPAPI.Service;
using APPDATA.Models;
using APPVIEW.Areas.Admin.Controllers;
using APPVIEW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using X.PagedList;

namespace APPVIEW.Controllers
{
    [Authorize(Roles ="Admin,Staff")]
    public class BillController : Controller
    {
        private Getapi<Bill> getapi;
        private Getapi<Account> getapiacc;
        private Getapi<Role> getrole;
        public BillController()
        {
            getapi = new Getapi<Bill>();
            getapiacc = new Getapi<Account>();
            getrole = new Getapi<Role>();
        }
  
        public async Task<IActionResult> GetList(string searchTerm, DateTime? start, DateTime? end, int? page)
        {
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var obj = getapi.GetApi("Bill").Where(c=>c.Type!= "Tại Quầy");
            ViewBag.account = getapiacc.GetApi("Account");
            try
            {
                if (!string.IsNullOrEmpty(searchTerm) && start == null && end == null)
                {
                    var tk = obj.Where(c => c.Code.ToLower().Contains(searchTerm.ToLower()) || c.Name.ToLower().Contains(searchTerm.ToLower()) || c.TotalMoney.ToString().Contains(searchTerm)).OrderByDescending(d => d.CreateDate).ToList();
                    return View(tk.ToPagedList(pageNumber, pageSize));
                }
                else if (string.IsNullOrEmpty(searchTerm) && start != null && end != null)
                {
                    // Adjust end date to include the entire day until 23:59:59
                    end = end?.Date.AddDays(1).AddTicks(-1); // Setting the end date to the end of the day

                    var tk = obj.Where(c => (c.CreateDate >= start && c.CreateDate <= end)).OrderByDescending(d => d.CreateDate).ToList();
                    return View(tk.ToPagedList(pageNumber, pageSize));
                }
                else if (string.IsNullOrEmpty(searchTerm) && start != null && end == null)
                {
                    // Filter bills for today
                    var todayStart = DateTime.Today;
                    var todayEnd = DateTime.Today.AddDays(1).AddTicks(-1);

                    var tk = obj.Where(c => (c.CreateDate >= todayStart && c.CreateDate <= todayEnd)).OrderByDescending(d => d.CreateDate).ToList();
                    return View(tk.ToPagedList(pageNumber, pageSize));
                }
                else if (!string.IsNullOrEmpty(searchTerm) && start != null && end != null)
                {
                    end = end?.Date.AddDays(1).AddTicks(-1);
                    var tk = obj.Where(c => (c.Code.ToLower().Contains(searchTerm.ToLower()) || c.TotalMoney.ToString().Contains(searchTerm)) && (c.CreateDate >= start && c.CreateDate <= end)).OrderByDescending(d => d.CreateDate).ToList();
                    return View(tk.ToPagedList(pageNumber, pageSize));
                }
                else
                {

                    return View(obj.OrderByDescending(d => d.CreateDate).ToPagedList(pageNumber, pageSize));

                }
            }
            catch (Exception ex)
            {
                return View(obj.OrderByDescending(d => d.CreateDate).ToPagedList(pageNumber, pageSize));
            }

        }
        public async Task<IActionResult> GetList2(string searchTerm, DateTime? start, DateTime? end,int?page)
        {
    
            if (User.Identity.IsAuthenticated)
            {

                var Uid = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                var acc = getapiacc.GetApi("Account").FirstOrDefault(c => c.Id.ToString() == Uid);
                SessionService.SetObjToJson(HttpContext.Session, "Account", acc);
                
            }
            var account = SessionService.GetUserFromSession(HttpContext.Session, "Account");
            var admmin = getrole.GetApi("Role").FirstOrDefault(c => c.name == "Admin");
            
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var obj = getapi.GetApi("Bill").Where(c => c.Type == "Tại Quầy"&& c.AccountId==account.Id);
            if (admmin != null) {
                if (account.IdRole == admmin.id) {

                    obj = getapi.GetApi("Bill").Where(c => c.Type == "Tại Quầy");
                }

            }
            
            ViewBag.account = getapiacc.GetApi("Account");

            try
            {
                if (!string.IsNullOrEmpty(searchTerm) && start == null && end == null)
                {
                    var tk = obj.Where(c => c.Code.ToLower().Contains(searchTerm.ToLower()) || c.Name.ToLower().Contains(searchTerm.ToLower()) || c.TotalMoney.ToString().Contains(searchTerm)).OrderByDescending(d => d.CreateDate).ToList();
                    return View(tk.ToPagedList(pageNumber, pageSize));
                }
                else if (string.IsNullOrEmpty(searchTerm) && start != null && end != null)
                {
                    // Adjust end date to include the entire day until 23:59:59
                    end = end?.Date.AddDays(1).AddTicks(-1); // Setting the end date to the end of the day

                    var tk = obj.Where(c => (c.CreateDate >= start && c.CreateDate <= end)).OrderByDescending(d => d.CreateDate).ToList();
                    return View(tk.ToPagedList(pageNumber, pageSize));
                }
                else if (string.IsNullOrEmpty(searchTerm) && start != null && end == null)
                {
                    // Filter bills for today
                    var todayStart = DateTime.Today;
                    var todayEnd = DateTime.Today.AddDays(1).AddTicks(-1);

                    var tk = obj.Where(c => (c.CreateDate >= todayStart && c.CreateDate <= todayEnd)).OrderByDescending(d => d.CreateDate).ToList();
                    return View(tk.ToPagedList(pageNumber, pageSize));
                }
                else if (!string.IsNullOrEmpty(searchTerm) && start != null && end != null)
                {
                    end = end?.Date.AddDays(1).AddTicks(-1);
                    var tk = obj.Where(c => (c.Code.ToLower().Contains(searchTerm.ToLower())|| c.TotalMoney.ToString().Contains(searchTerm)) && (c.CreateDate >= start && c.CreateDate <= end)).OrderByDescending(d => d.CreateDate).ToList();
                    return View(tk.ToPagedList(pageNumber, pageSize));
                }
                else
                {

                    return View(obj.OrderByDescending(d => d.CreateDate).ToPagedList(pageNumber, pageSize));

                }
            }
            catch (Exception ex)
            {

                return View(obj.OrderByDescending(d => d.CreateDate).ToPagedList(pageNumber, pageSize));

            }
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
