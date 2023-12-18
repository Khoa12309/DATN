using APPDATA.Models;
using APPVIEW.Services;
using APPVIEW.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using _APPAPI.Service;
using AspNetCoreHero.ToastNotification.Abstractions;
using APPDATA.DB;
using System.Text.Encodings.Web;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Rewrite;
using System.Xml.Linq;

using X.PagedList;

using APPVIEW.Models;
using _APPAPI.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace APPVIEW.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private Getapi<Account> getapi;
        private Getapi<Role> _getapiRole;
        private Getapi<Address> getapiAddress;
        private readonly ISendEmail _sendEmail;
        private readonly ShoppingDB _context;
        private readonly SendEmailMessage _sendEmailMessage;
        private readonly ShoppingDB _dbContext;
        public INotyfService _notyf;
        public AccountController(HttpClient httpClient, ISendEmail sendEmail, INotyfService notyf)
        {
            getapi = new Getapi<Account>();
            _httpClient = httpClient;
            _getapiRole = new Getapi<Role>();
            getapiAddress = new Getapi<Address>();
            _sendEmail = sendEmail;
            _context = new ShoppingDB();
            _sendEmailMessage = new SendEmailMessage();
            _dbContext = new ShoppingDB();
            _notyf = notyf;
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetList(int? page)

        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.Roles = GetListRole();
                // Nếu là Admin, hiển thị tất cả các tài khoản

                var cusRoleId = _dbContext.Roles.FirstOrDefault(c => c.name == "Customer")?.id;
                var staffId = _dbContext.Roles.FirstOrDefault(c => c.name == "Staff")?.id;
                var adminId = _dbContext.Roles.FirstOrDefault(c => c.name == "Admin")?.id;
                if (cusRoleId != null && staffId != null && adminId != null)
                {


                    // Đếm số lượng khách hàng đã đăng ký trong tháng này
                    ViewBag.CountUser = await _dbContext.Accounts
                        .Where(c => c.Status != 2 && c.IdRole == cusRoleId)
                        .CountAsync();
                    ViewBag.CountActive = await _dbContext.Accounts
                        .Where(c => c.Status == 1 && c.IdRole == cusRoleId)
                        .CountAsync();
                    ViewBag.CountInActive = await _dbContext.Accounts
                        .Where(c => c.Status == 0 && c.IdRole == cusRoleId)
                        .CountAsync();
                    ViewBag.CountBlocked = await _dbContext.Accounts
                    .Where(c => c.Status == 2 && c.IdRole == cusRoleId)
                    .CountAsync();

                    ViewBag.CountRole = await _dbContext.Roles.Where(c => c.Status == 1).CountAsync();
                }

                var roleActive = GetListRole().FirstOrDefault(c => c.Status == 0);
                if (roleActive != null)
                {
                    var obj = getapi.GetApi("Account");

                    int pageSize = 8;
                    int pageNumber = page ?? 1;
                    return View(obj.Where(c => c.Status != 2 && c.Status != 3 && c.IdRole != roleActive.id).ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    var obj = getapi.GetApi("Account");
                    int pageSize = 8;
                    int pageNumber = page ?? 1;
                    return View(obj.Where(c => c.Status != 2 && c.Status != 3).ToPagedList(pageNumber, pageSize));
                }
            }
            else if (User.IsInRole("Staff"))
            {
                var cusRoleId = _dbContext.Roles.FirstOrDefault(c => c.name == "Customer")?.id;
                ViewBag.CountUser = await _dbContext.Accounts
                      .Where(c => c.Status != 2 && c.IdRole == cusRoleId)
                      .CountAsync();
                ViewBag.CountActive = await _dbContext.Accounts
                    .Where(c => c.Status == 1 && c.IdRole == cusRoleId)
                    .CountAsync();
                ViewBag.CountInActive = await _dbContext.Accounts
                    .Where(c => c.Status == 0 && c.IdRole == cusRoleId)
                    .CountAsync();
                ViewBag.CountBlocked = await _dbContext.Accounts
                .Where(c => c.Status == 2 && c.IdRole == cusRoleId)
                .CountAsync();
                ViewBag.Roles = GetListRole();
                var roleActive = GetListRole().FirstOrDefault(c => c.Status == 0);
                if (roleActive != null)
                {
                    var CustomerRole = GetListRole().FirstOrDefault(c => c.name == "Customer").id;
                    // Nếu là Staff, chỉ hiển thị tài khoản Customer
                    var obj = getapi.GetApi("Account").Where(c => c.IdRole == CustomerRole);
                    int pageSize = 8;
                    int pageNumber = page ?? 1;
                    return View(obj.Where(c => c.Status != 2 && c.Status != 3 && c.IdRole != roleActive.id).ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    var CustomerRole = GetListRole().FirstOrDefault(c => c.name == "Customer").id;
                    // Nếu là Staff, chỉ hiển thị tài khoản Customer
                    var obj = getapi.GetApi("Account").Where(c => c.IdRole == CustomerRole);
                    int pageSize = 8;
                    int pageNumber = page ?? 1;
                    return View(obj.Where(c => c.Status != 2 && c.Status != 3).ToPagedList(pageNumber, pageSize));
                }



            }

            return View();


        }
        [HttpPost]
        public async Task<IActionResult> GetList(int? page, string tk, string status, Guid role)
        {
            if (User.IsInRole("Staff"))
            {
                ViewBag.Roles = GetListRole();
                var Cusrole = GetListRole().FirstOrDefault(c => c.name == "Customer").id;
                var AdminRole =  _dbContext.Roles.FirstOrDefault(c => c.name == "Admin").id;
                var StaffRole = _dbContext.Roles.FirstOrDefault(c => c.name == "Staff").id;
                var obj = getapi.GetApi("Account").Where(c => c.Status != 3);
                if (tk != null)
                {
                    // Loại bỏ tài khoản admin bằng cách kiểm tra vai trò
                    obj = obj.Where(c => (c.Name.ToLower().Contains(tk.ToLower()) || c.Email == tk)
                                && c.IdRole != AdminRole && c.IdRole != StaffRole).ToList();
                }
                if (role != Guid.Empty)
                {
                    obj = obj.Where(c => c.IdRole == role).ToList();
                }
                if (status != null)
                {
                    obj = obj.Where(c => c.Status.ToString() == status && c.IdRole != AdminRole && c.IdRole != StaffRole).ToList();
                }
                int pageSize = 8;
                int pageNumber = (page ?? 1);
                return View(obj.Where(c => c.Status != 2 && c.Status != 3 && c.IdRole != AdminRole).ToPagedList(pageNumber, pageSize));
            }
            else
            {
                ViewBag.Roles = GetListRole();
                var roleActive = GetListRole().FirstOrDefault(c => c.Status == 0);
                if (roleActive != null)
                {

                }
                var obj = getapi.GetApi("Account").Where(c => c.Status != 3);
                if (tk != null)
                {
                    obj = obj.Where(c => c.Name.ToLower().Contains(tk.ToLower()) || c.Email == tk).ToList();

                }
                if (role != Guid.Empty)
                {
                    obj = obj.Where(c => c.IdRole == role).ToList();
                }
                if (status != null)
                {
                    obj = obj.Where(c => c.Status.ToString() == status).ToList();
                }
                int pageSize = 8;
                int pageNumber = (page ?? 1);
                return View(obj.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
            }


        }

        public async Task<IActionResult> Search(string tk, int? page)
        {
            // Kiểm tra vai trò của người dùng
            if (User.IsInRole("Staff"))
            {
                // Lấy ID của vai trò "Customer"
                Guid customerRoleId = GetListRole().FirstOrDefault(c => c.name == "Customer").id;

                // Lấy danh sách tài khoản chỉ có vai trò "Customer"
                var lstAcc = getapi.GetApi("Account").Where(c => c.Name.ToLower().Contains(tk.ToLower()) && c.IdRole == customerRoleId);

                if (lstAcc.Any())
                {
                    var searchResult = lstAcc
                        .Where(v => v.Name.ToLower().Contains(tk.ToLower()))
                        .ToList();
                }
                else
                {
                    _notyf.Warning("Không có dữ liệu");
                }

                int pageSize = 8;
                int pageNumber = page ?? 1;
                return RedirectToAction("Getlist", lstAcc.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
            }
            else
            {
                // Nếu không phải Staff, tìm kiếm tất cả các tài khoản
                var lstAcc = getapi.GetApi("Account").Where(c => c.Name.ToLower().Contains(tk.ToLower()));

                var searchResult = lstAcc
                    .Where(v => v.Name.ToLower().Contains(tk.ToLower()))
                    .ToList();

                int pageSize = 8;
                int pageNumber = page ?? 1;
                return RedirectToAction("Getlist", lstAcc.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
            }
        }



        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost, AllowAnonymous]

        public async Task<IActionResult> Register(RegisterVm obj)
        {
            var checkEmail = await _context.Accounts.FirstOrDefaultAsync(c => c.Email == obj.Email);
            var checkPhone = await _context.Address.FirstOrDefaultAsync(c => c.PhoneNumber == obj.PhoneNumber);
            if (string.IsNullOrEmpty(obj.Email) || string.IsNullOrEmpty(obj.Name) || string.IsNullOrEmpty(obj.ConfirmPassword))
            {

                ViewData["ErrorMessage"] = "Vui lòng nhập số tài khoản.";
                return View("Register", obj);

            }
            if (obj.Password != obj.ConfirmPassword)
            {
                ViewData["ErrorMessage"] = "Mật khẩu xác nhận và mật khẩu không khớp nhau,hãy thử lại!";
                return View("Register", obj);
            }
            else if (checkEmail != null)
            {
                ViewData["ErrorMessage"] = "Email này đã được đăng kí, hãy tạo tài khoản bằng email khác để đăng kí!";
                return View("Register", obj);
            }
            else if (checkPhone != null)
            {
                ViewData["ErrorMessage"] = "Số điện thoại này đã được đăng kí !";
                return View("Register", obj);
            }

            var md5pass = MD5Pass.GetMd5Hash(obj.Password);
            obj.ResetPasswordcode = RundomCodeService.GenerateRandomCode(6);

            obj.Password = md5pass;
            var jsonData = JsonConvert.SerializeObject(obj);

            var address = new Address()
            {
                id = Guid.NewGuid(),
                AccountId = obj.Id,
                City = "",
                Ward = "",
                District = "",
                PhoneNumber = obj.PhoneNumber,
                Description = "",
                Name = obj.Name,
                Province = "",
                DefaultAddress = "",
                SpecificAddress = ""


            };
            var jsonDataAddress = JsonConvert.SerializeObject(address);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var contentAddress = new StringContent(jsonDataAddress, Encoding.UTF8, "application/json");



            var responese = await _httpClient.PostAsync("https://localhost:7042/api/Account/Register", content);
            var responeseAdress = await _httpClient.PostAsync("https://localhost:7042/api/Address/Post", contentAddress);
            if (responese.IsSuccessStatusCode && responeseAdress.IsSuccessStatusCode)
            {

                // Gửi email với đường link xác nhận
                var tokenLink = Url.Action("ConfirmEmail", "Account", new { email = obj.Email, token = obj.ResetPasswordcode }, Request.Scheme);
                _sendEmail.SendEmailAsync(obj.Email, "Confirm your email", $"Please confirm your email by clicking <a href='{tokenLink}'>here</a>.");
                ViewData["Sucsess"] = "Chúng tôi đã gửi thư xác nhận đến email của bạn hãy xác thực để có thể đăng nhập!";
                return View("Login");

            }
            else
            {
                var errorResponse = await responese.Content.ReadAsStringAsync();
                return View();
            }
        }
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            // Lấy token từ cơ sở dữ liệu hoặc cache
            var acc = await _dbContext.Accounts.SingleOrDefaultAsync(c => c.Email == email);
            var savedToken = acc.ResetPasswordcode;

            // Kiểm tra xem token từ đường link có khớp với token đã lưu trữ hay không
            if (token == savedToken)
            {
                // Xác nhận thành công, có thể đánh dấu tài khoản đã được xác nhận trong cơ sở dữ liệu
                acc.Status = 1;
                acc.ResetPasswordcode = null;
                _context.Accounts.Update(acc);
                await _context.SaveChangesAsync();

                // Chuyển hướng đến trang thông báo thành công hoặc trang đăng nhập
                return View("Login");
            }
            else
            {
                // Xử lý lỗi: Token không hợp lệ hoặc đã hết hạn
                return Redirect("~/Error/AccessDenied");
            }
        }

        [HttpGet, AllowAnonymous]

        public IActionResult Login(string ReturnUrl)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }
        [HttpPost, AllowAnonymous]

        public async Task<IActionResult> Login(LoginVm obj, string ReturnUrl)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            if (string.IsNullOrWhiteSpace(obj.Email) || string.IsNullOrWhiteSpace(obj.Password))
            {
                ViewData["ErrorMessage"] = "Please enter your email and password.";
                return View("Login", obj);
            }

            var md5pass = MD5Pass.GetMd5Hash(obj.Password);
            obj.Password = md5pass;
            var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7042/api/Account/Login", content);

            if (response.IsSuccessStatusCode)
            {

                var responseData = await response.Content.ReadAsStringAsync();
                var loginResult = JsonConvert.DeserializeObject<TokenVm>(responseData);
                TokenVm tokenV = new TokenVm { AccessToken = loginResult.AccessToken };
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(loginResult.AccessToken);

                bool checkRoleAdmin = false;
                bool checkRoleStaff = false;
                var checkRoles = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                if (checkRoles.StartsWith("Adm"))
                {
                    checkRoleAdmin = true;
                }
                else if (checkRoles.StartsWith("Sta"))
                {
                    checkRoleStaff = true;
                }
                else
                {
                    checkRoleAdmin = false;
                }

                //Check Role and Get information of user
                var claims = new List<Claim>
                   {
                    new Claim(ClaimTypes.Email, obj.Email),

                    };
                // Trích xuất thông tin quyền từ mã thông báo JWT
                var roles = jwt.Claims.ToList();

                //    Thêm các quyền từ mã thông báo JWT vào danh tính của người dùng
                if (roles.Any())
                {
                    foreach (var role in roles)
                    {

                        if (role.Type.ToString() == "role")
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.Value));

                        }
                        if (role.Type.ToString() == "Id")
                        {
                            var acc = getapi.GetApi("Account").FirstOrDefault(c => c.Id.ToString() == role.Value);
                            SessionService.SetObjToJson(HttpContext.Session, "Account", acc);
                        }

                    }

                }
                var Avatar = jwt.Claims.FirstOrDefault(c => c.Type == "Avatar")?.Value;
                var Name = jwt.Claims.FirstOrDefault(c => c.Type == "Name")?.Value;
                var Id_User = jwt.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (Id_User != null)
                {
                    claims.Add(new Claim("Id", Id_User.ToString()));
                    claims.Add(new Claim("Avatar", Avatar.ToString()));
                    claims.Add(new Claim("Name", Name));
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                Response.Cookies.Append("AccessToken", loginResult.AccessToken);
                if (checkRoleAdmin == true)
                {
                    return Redirect(!string.IsNullOrEmpty(ViewData["ReturnUrl"]?.ToString()) ? ViewData["ReturnUrl"].ToString() : "~/Admin/Admin/Index");

                }
                else if (checkRoleStaff == true)
                {
                    return Redirect(!string.IsNullOrEmpty(ViewData["ReturnUrl"]?.ToString()) ? ViewData["ReturnUrl"].ToString() : "~/QLBills/BanHangOff");
                }
                else
                {
                    return Redirect(!string.IsNullOrEmpty(ViewData["ReturnUrl"]?.ToString()) ? ViewData["ReturnUrl"].ToString() : "~/Home/Index");
                }



            }

            ViewData["ErrorMessage"] = "Email hoặc mật khẩu đã sai vui lòng nhập lại.";
            return View("Login", obj);

        }
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.Roles = GetListRole();
            var lst = getapi.GetApi("Account");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost, Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(Account obj, [Bind] IFormFile imageFile)
        {
            try
            {
                if (imageFile != null)
                {
                    obj.Avatar = AddImg(imageFile);
                }
                await getapi.UpdateObj(obj, "Account");
                _notyf.Success("Edit Sucsess");
                return RedirectToAction("GetList");

            }
            catch
            {
                _notyf.Error("Error");
                return View();
            }
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var acc = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (acc != null)
            {
                var add = await _context.Address.FirstOrDefaultAsync(c => c.AccountId == id);
                if (add != null)
                {
                    if (add.Status == 2 && acc.Status == 2)
                    {
                        acc.Status = 1;
                        add.Status = 1;
                        _context.Update(acc);
                        _context.Update(add);
                        await _context.SaveChangesAsync();
                        _notyf.Success("Đã đổi trạng thái tài khoản thành công!");
                        return RedirectToAction(nameof(GetList));
                    }
                }
                else
                {
                    _notyf.Error("Lỗi");
                }


                acc.Status = 2;
                add.Status = 2;
                _context.Update(acc);
                _context.Update(add);
                await _context.SaveChangesAsync();
                //_sendEmail.SendEmailAsync(acc.Email, "Khóa tài khoản", _sendEmailMessage.SendEmailBlock(acc.Name, acc.Email));
                _notyf.Success($"Đã khóa tài khoản:{acc.Name}");
                return RedirectToAction(nameof(GetList));
            }
            _notyf.Error("Error");
            return View();
        }
        [HttpGet, Authorize(Roles = "Admin,Staff,Customer")]

        public async Task<IActionResult> Logout()
        {

            Response.Cookies.Delete("AccessToken");

            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            return Redirect("~/Home/Index");
        }

        [HttpGet, Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> MyProfile(Guid id_User)
        {
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");
            // Lấy thông tin voucher từ TempData
            var discountAmountString = TempData["DiscountAmount"] as string;
            var voucherCode = TempData["VoucherCode"] as string;
            // Gọi API để lấy danh sách các tỉnh/thành phố
            var response = await client.GetProvincesAsync();
            if (response.Code == 200) // Thành công
            {
                // Trả về danh sách các quận/huyện dưới dạng JSON
                ViewBag.province = response.Data;
            }

            var obj = getapi.GetApi("Account");
            var address = getapiAddress.GetApi("Address");

            var user = obj.FirstOrDefault(c => c.Id == id_User);
            var addressOfUser = address.FirstOrDefault(c => c.AccountId == id_User);

            if (addressOfUser != null || user != null)
            {
                return View(new AccountVm()
                {
                    Id = addressOfUser.id,
                    Avatar = user.Avatar,
                    Name = user.Name,
                    Email = user.Email,
                    Password = user.Password,
                    AccountId = id_User,
                    SpecificAddress = addressOfUser.SpecificAddress,
                    Ward = addressOfUser.Ward,
                    City = addressOfUser.City,
                    District = addressOfUser.District,
                    PhoneNumber = addressOfUser.PhoneNumber,
                    DefaultAddress = addressOfUser.DefaultAddress,
                    Province = addressOfUser.Province,
                    Description = addressOfUser.Description,
                    Id_Role = user.IdRole,


                });
            }
            return View();

        }
        public string AddImg(IFormFile imageFile)
        {


            if (imageFile != null && imageFile.Length > 0) // Không null và không trống
            {
                //Trỏ tới thư mục wwwroot để lát nữa thực hiện việc Copy sang
                var path = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot", "UserImage", imageFile.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    // Thực hiện copy ảnh vừa chọn sang thư mục mới (wwwroot)
                    imageFile.CopyTo(stream);
                }
                // Gán lại giá trị cho Description của đối tượng bằng tên file ảnh đã được sao chép

            }
            return imageFile.FileName;


        }
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> MyProfile(AccountVm obj, [Bind] IFormFile imageFile)
        {
            try
            {

                if (obj != null)
                {
                    var p = Convert.ToInt32(obj.Province);
                    if (p != 0)
                    {
                        var d = await dis(obj.District, p);
                        if (d != 0)
                        {
                            var w = await wad(obj.Ward, d);
                            if (w == 0) // Thành công
                            {
                                // Trả về danh sách các quận/huyện dưới dạng JSON
                                _notyf.Warning("Phường/xã không đúng");
                                return Redirect($"~/Account/MyProfile?id_User={obj.AccountId}");


                            }
                        }
                        else
                        {
                            _notyf.Warning("Quận/huyện không đúng");
                            return Redirect($"~/Account/MyProfile?id_User={obj.AccountId}");

                        }
                    }
                }
                else
                {
                    _notyf.Warning("Phường/xã không đúng");
                    return Redirect($"~/Account/MyProfile?id_User={obj.AccountId}");

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
                var user = new Account();
                user.Email = obj.Email;
                if (imageFile != null)
                {
                    user.Id = obj.AccountId;
                    user.Name = obj.Name;
                    user.Password = obj.Password;
                    user.IdRole = obj.Id_Role;
                    user.Avatar = AddImg(imageFile);
                }
                var address = new Address()
                {
                    AccountId = obj.AccountId,
                    Province = obj.Province,
                    DefaultAddress = obj.DefaultAddress,
                    SpecificAddress = obj.SpecificAddress,
                    City = obj.City,
                    District = obj.District,
                    PhoneNumber = obj.PhoneNumber,
                    Ward = obj.Ward,
                    Name = obj.Name,
                    id = obj.Id,
                    Description = obj.Description

                };

                var responeseAcc = await getapi.UpdateObj(user, "Account");
                var responeseAdd = await getapiAddress.UpdateObj(address, "Address");
                return Redirect($"~/Account/MyProfile?id_User={obj.AccountId}");
            }
            catch
            {
                return View();
            }
        }
        public List<Role> GetListRole()
        {
            if (User.IsInRole("Staff"))
            {
                var obj = _getapiRole.GetApi("Role").Where(c => c.name != "Admin" && c.name != "Staff");
                return obj.ToList();
            }
            else
            {
                var obj = _getapiRole.GetApi("Role");
                return obj;
            }






        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ResetPass resetPass)
        {
            if (string.IsNullOrWhiteSpace(resetPass.Email))
            {
                ViewData["ErrorMessage"] = "Email bắt buộc!";
                return View(resetPass);

            }
            var user = _context.Accounts.FirstOrDefault(u => u.Email == resetPass.Email);

            if (user != null)
            {
                var resetToken = RundomCodeService.GenerateRandomCode(6);
                user.ResetPasswordcode = resetToken;
                await _context.SaveChangesAsync();

                // Send reset link  email
                var resetLink = Url.Action("ResetPass", "Account", new { }, Request.Scheme);
                var emailSubject = "Yêu cầu đặt lại mật khẩu";
                var emailBody = $"Chào {user.Name}, <br/> Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản của bạn.Đây là mã đặt lại: <b>{resetToken}</b> Vui lòng click vào đừng dẫn này để có thể đặt lại mật khẩu tài khoản: <a href='{HtmlEncoder.Default.Encode(resetLink)}'>Reset Password</a>";

                await _sendEmail.SendEmailAsync(user.Email, emailSubject, emailBody);

                ViewData["Sucsess"] = "Mã đặt lại tài khoản đã được gửi đến Email của bạn,Hãy kiểm tra Email của bạn";
                return View(resetPass);


            }
            else
            {
                ViewData["ErrorMessage"] = "Không tìm thấy địa chỉ Email này!";
                return View(resetPass);
            }

        }
        public IActionResult ResetPass()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPass(ResetPasswordVm obj)
        {
            if (string.IsNullOrWhiteSpace(obj.ConfirmCode) || string.IsNullOrWhiteSpace(obj.ConfirmPassword) || string.IsNullOrWhiteSpace(obj.NewPassword))
            {
                ViewData["ErrorMessage"] = "Vui lòng nhập mật khẩu và mật khẩu mới của bạn";
                return View(obj);

            }
            if (obj.NewPassword != obj.ConfirmPassword)
            {
                ViewData["ErrorMessage"] = "Mật khẩu mới và mật khẩu xác nhận của bạn không trùng khớp với nhau vui lòng thử lại!";
                return View(obj);
            }
            else
            {
                var user = _context.Accounts.FirstOrDefault(s => s.Email == obj.Email);
                if (user != null)
                {
                    if (obj.ConfirmCode != user.ResetPasswordcode)
                    {
                        ViewData["ErrorMessage"] = "Mã xác nhận không đúng!";
                        return View(obj);
                    }
                    else
                    {
                        user.Password = MD5Pass.GetMd5Hash(obj.NewPassword);
                        user.ResetPasswordcode = null;
                        await _context.SaveChangesAsync();
                        ViewData["Sucsess"] = "Đổi mật khẩu thành công,bây giờ bạn có thể quay lại trang đăng nhập!";
                        return View(obj);
                    }

                }
                else
                {
                    ViewData["ErrorMessage"] = "Không tìm thấy địa chỉ Email!";
                    return View(obj);
                }

            }

        }
        public IActionResult ChangePassword()
        {

            return View();
        }
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ChangePassword(ChangePasswordVm obj)
        {
            if (ModelState.IsValid)
            {
                var IdUser = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst("Id");
                string Id_userValue = IdUser?.Value;
                obj.IdUser = Guid.Parse(Id_userValue);

                var user = await _context.Accounts.FirstOrDefaultAsync(c => c.Id == obj.IdUser);
                if (user != null)
                {
                    if (MD5Pass.GetMd5Hash(obj.OldPassword) != user.Password)
                    {
                        ViewData["ErrorMessage"] = "Mật khẩu cũ không đúng";
                        return View("ChangePassword", obj);
                    }
                    if (obj.NewPassWord != obj.ConfirmPassword)
                    {
                        ViewData["ErrorMessage"] = " Mật khẩu không khớp với mật khẩu mới";
                        return View("ChangePassword", obj);
                    }

                    user.Password = MD5Pass.GetMd5Hash(obj.NewPassWord);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    ViewData["Sucsess"] = "Thay đổi mật khẩu thành công";
                    return Redirect("~/Home/Index");


                }
            }
            _notyf.Error("Error");
            return View();
        }
        public IActionResult Create()
        {
            ViewBag.ListRole = GetListRole().Where(c => c.Status != 0).ToList();
            return View();
        }
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RegisterVm obj)
        {

            if (string.IsNullOrEmpty(obj.Email) || string.IsNullOrEmpty(obj.Name))
            {

                _notyf.Warning("Không được để trống!");
                return View("Create", obj);

            }
            var md5pass = MD5Pass.GetMd5Hash(obj.PhoneNumber);

            obj.Password = md5pass;

            var acc = new Account()
            {
                Id = obj.Id,
                Email = obj.Email,
                IdRole = obj.Id_Role,
                Name = obj.Name,
                Password = md5pass,
                Avatar = string.Empty

            };
            var jsonData = JsonConvert.SerializeObject(acc);

            var address = new Address()
            {
                id = Guid.NewGuid(),
                AccountId = obj.Id,
                City = "N/A",
                Ward = "N/A",
                District = "N/A",
                PhoneNumber = obj.PhoneNumber,
                Description = "N/A",
                Name = obj.Name,
                Province = "N/A",
                DefaultAddress = "N/A",
                SpecificAddress = "N/A"


            };
            var jsonDataAddress = JsonConvert.SerializeObject(address);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var contentAddress = new StringContent(jsonDataAddress, Encoding.UTF8, "application/json");


            var responese = await _httpClient.PostAsync("https://localhost:7042/api/Account/Register", content);
            var responeseAdress = await _httpClient.PostAsync("https://localhost:7042/api/Address/Post", contentAddress);
            if (responese.IsSuccessStatusCode && responeseAdress.IsSuccessStatusCode)
            {

              string Subject = "Create account successfully";
              await  _sendEmail.SendEmailAsync(obj.Email, Subject, _sendEmailMessage.SendEmail(obj.Name, obj.Email, obj.PhoneNumber));

                _notyf.Success($"Tạo tài khoản cho: {obj.Name} Thành công và đã gửi Email đến địa chỉ: {obj.Email}!");
                return Redirect("~/Account/GetList");

            }
            else
            {
                var errorResponse = await responese.Content.ReadAsStringAsync();
                _notyf.Error($"Error : {errorResponse} ");
                return View();
            }

        }
        [HttpGet, Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> AccountBlockedAll()
        {
            if (User.IsInRole("Staff"))
            {
                ViewBag.Roles = GetListRole();
                Guid customer = _context.Roles.FirstOrDefault(c => c.name == "Customer").id;
                var lst = getapi.GetApi("Account");
                if (lst != null)
                {
                    return View(lst.Where(c => c.Status == 2 && c.IdRole == customer).ToList());
                }
                return View();
            }
            else
            {
                ViewBag.Roles = GetListRole();
                var lst = getapi.GetApi("Account");
                if (lst != null)
                {
                    return View(lst.Where(c => c.Status == 2).ToList());
                }
                return View();
            }



        }
        [HttpGet, Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> AccountDetails(Guid id)
        {
            ViewBag.Roles = GetListRole().Where(c => c.Status != 0).ToList();
            var acc = await getapi.GetApia("Account");
            var add = await getapiAddress.GetApia("Address");

            try
            {
                if (acc != null)
                {

                    var addfind = add.FirstOrDefault(c => c.AccountId == id);
                    var accfind = acc.FirstOrDefault(c => c.Id == id);
                    return View(new AccountVm()
                    {
                        Name = accfind.Name,
                        Id_Role = accfind.IdRole,
                        PhoneNumber = addfind.PhoneNumber,
                        AccountId = accfind.Id,
                        Avatar = accfind.Avatar,
                        Email = accfind.Email,
                        Province = addfind.Province,
                        DefaultAddress = addfind.DefaultAddress,
                        District = addfind.District,
                        SpecificAddress = addfind.SpecificAddress,
                        Ward = addfind.Ward,
                        Status = accfind.Status
                    });
                }
                else
                {
                    _notyf.Error("Lỗi");
                    return View();
                }

            }
            catch (Exception)
            {

                _notyf.Error("Lỗi");
                return View();
            }
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
                    if (item.NameExtension.Any(c => c.Contains(ten)) || item.DistrictName.ToLower() == ten.ToLower())
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
    }
}
