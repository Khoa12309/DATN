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

namespace APPVIEW.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private Getapi<Account> getapi;
        public AccountController(HttpClient httpClient)
        {
            getapi = new Getapi<Account>();
            _httpClient = httpClient;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Account");
            return View(obj);
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
            if (string.IsNullOrEmpty(obj.Email) || string.IsNullOrEmpty(obj.Name) || string.IsNullOrEmpty(obj.ConfirmPassword))
            {

                return RedirectToAction("Register", "Account");

            }
            if (obj.Password != obj.ConfirmPassword)
            {
                return RedirectToAction("Register", "Account");
            }
            var md5pass = MD5Pass.GetMd5Hash(obj.Password);

            obj.Password = md5pass;
            var jsonData = JsonConvert.SerializeObject(obj);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responese = await _httpClient.PostAsync("https://localhost:7042/api/Account/Register", content);
            if (responese.IsSuccessStatusCode)
            {
                // Đăng nhập người dùng sau khi đăng ký thành công (nếu cần)
                // Xử lý hành động sau khi đăng ký thành công (chẳng hạn chuyển hướng đến trang chính)
                //  _notyf.Success("Đăng ký tài khoản thành công!");
                return Redirect("~/Account/Login");

            }
            else
            {
                // Xử lý lỗi trả về từ API (nếu có)
                var errorResponse = await responese.Content.ReadAsStringAsync();
                //  _notyf.Error("Đăng ký thất bại: " + errorResponse);
                return View();
            }
        }
        [HttpGet, AllowAnonymous]

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost, AllowAnonymous]

        public async Task<IActionResult> Login(LoginVm obj)
        {
            if (string.IsNullOrEmpty(obj.Email) || string.IsNullOrEmpty(obj.Password))
            {
                //  _notyf.Error("Vui lòng nhập email và mật khẩu.");
                return RedirectToAction("Login", "Account");
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


                //Check Role and Get information of user
                var claims = new List<Claim>
                   {
                    new Claim(ClaimTypes.Name, obj.Email), 
                    // Thêm thông tin khác của người dùng nếu cần
                    };

                // Trích xuất thông tin quyền từ mã thông báo JWT
                var roles = jwt.Claims.ToList();


                //Lưu User vào session
                

               

                bool checkRoleAdmin = false;
                // Trích xuất thông tin quyền từ mã thông báo JWT


                //    Thêm các quyền từ mã thông báo JWT vào danh tính của người dùng
                if (roles.Any())
                {
                    foreach (var role in roles)
                    {
                       
                        if (role.Type.ToString() == "role")
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.Value));
                            checkRoleAdmin = true;                         
                        }
                        if (role.Type.ToString() == "Id")
                        {
                            var acc = getapi.GetApi("Account").FirstOrDefault(c => c.Id.ToString() == role.Value);
                            SessionService.SetObjToJson(HttpContext.Session, "Account",acc);
                        }

                    }
                    if (checkRoleAdmin == false)
                    {
                        // Nếu không có quyền từ mã thông báo JWT, thêm quyền mặc định "Customer"
                        claims.Add(new Claim(ClaimTypes.Role, "Customer"));
                    }

                }
                var customData = jwt.Claims.FirstOrDefault(c => c.Type == "Avatar")?.Value;
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                Response.Cookies.Append("AccessToken", loginResult.AccessToken);
                if (checkRoleAdmin == true)
                {
                    return Redirect("~/Address/GetList");
                   // return Redirect("~/Home/Index");
                }
                else
                {
                    // _notyf.Success($"Login success! Welcome {obj.Email}");
                    return Redirect("~/Home/Index");
                  //  return Redirect("~/Address/GetList");
                }



            }

            //  _notyf.Error($"Error: {response.StatusCode.ToString()}!");
            return BadRequest("Đăng nhập thất bại");

        }
        public async Task<IActionResult> Edit(Guid id)
        {

            var lst = getapi.GetApi("Account");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Account obj)
        {
            try
            {
                await getapi.UpdateObj(obj, "Account");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await getapi.DeleteObj(id, "Account");
            return RedirectToAction("GetList");

        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {

            Response.Cookies.Delete("AccessToken");
            SessionService.Clearobj(HttpContext.Session, "Xoa");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            return Redirect("~/Account/Login");
        }

    }
}
