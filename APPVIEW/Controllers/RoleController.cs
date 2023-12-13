using APPDATA.DB;
using APPDATA.Models;
using APPVIEW.Services;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private Getapi<Role> getapi;
        private Getapi<Account> _getAcc;
        private ShoppingDB _dbContext;
        public  INotyfService _notyf { get; }

        public RoleController(INotyfService notyff)
        {
            _getAcc = new Getapi<Account>();
            getapi = new Getapi<Role>();
            _notyf = notyff;
            _dbContext = new ShoppingDB();
        }
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
               
            var obj =  _getAcc.GetApi("Account");
            if (obj!=null)
            {
                return View(obj);
            }
            return View();
        }
        public async Task<IActionResult> GetList()
        {
            // Lấy ID của vai trò "Customer"
            var cusRoleId = _dbContext.Roles.FirstOrDefault(c => c.name == "Customer")?.id;
            var staffId = _dbContext.Roles.FirstOrDefault(c => c.name == "Staff")?.id;
            var adminId = _dbContext.Roles.FirstOrDefault(c => c.name == "Admin")?.id;

            if (cusRoleId != null&&staffId!=null&&adminId!=null)
            {
                // Lấy ngày bắt đầu của tháng hiện tại
                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                // Đếm số lượng khách hàng đã đăng ký trong tháng này
                ViewBag.CountUser = await _dbContext.Accounts
                    .Where(c => c.Status != 2 && c.IdRole == cusRoleId && c.Create_date >= startDate)
                    .CountAsync();
                ViewBag.CountStaff = await _dbContext.Accounts
                    .Where(c => c.Status != 2 && c.IdRole == staffId)
                    .CountAsync();
                ViewBag.CountAdmin = await _dbContext.Accounts
                    .Where(c => c.Status != 2 && c.IdRole == adminId)
                    .CountAsync();
                ViewBag.CountRole = await _dbContext.Roles.Where(c=>c.Status==1).CountAsync();
            }
           
            var obj = getapi.GetApi("Role");
            return View(obj);
        }      


        public async Task<IActionResult> Search(string searchTerm)
        {
            var lstRole = getapi.GetApi("Role").ToList();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View("GetList", lstRole);
            }
            var searchResult = lstRole
                .Where(v =>
                    v.name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
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
        public async Task<IActionResult> Create(Role obj)
        {


            try
            {
                var item = getapi.CreateObj(obj, "Role").Result;
                if (item != null)
                {
                    _notyf.Success("Thêm thành công!");
                    return RedirectToAction("GetList");
                }
                else
                {
                    _notyf.Warning("Không được để trống!");
                    return View();
                }
            }
            catch (Exception)
            {

                return View();
            }




        }
        [HttpGet]


        public async Task<IActionResult> Edit(Guid id)
        {

            var lst = getapi.GetApi("Role");
            return View(lst.Find(c => c.id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Role obj)
        {
            try
            {
                var item = getapi.UpdateObj(obj, "Role").Result;
                if (item != null)
                {
                    _notyf.Success("Edit thành công!");
                    return RedirectToAction("GetList");
                }
                else
                {
                    _notyf.Warning("Không được để trống!");
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            await getapi.DeleteObj(id, "Role");
            return RedirectToAction("GetList");

        }
    }
}
