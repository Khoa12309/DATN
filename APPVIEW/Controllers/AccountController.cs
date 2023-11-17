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

namespace APPVIEW.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private Getapi<Account> getapi;
        private Getapi<Role> _getapiRole;
        private Getapi<Address> getapiAddress;
        private readonly INotyfService _notyf;
        public AccountController(HttpClient httpClient, INotyfService notyf)
        {
            getapi = new Getapi<Account>();
            _httpClient = httpClient;
            _notyf = notyf;
            _getapiRole = new Getapi<Role>();
            getapiAddress = new Getapi<Address>();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetList()
        {
            ViewBag.Roles = GetListRole();
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

        public async Task<IActionResult> Register(RegisterVm obj, Account item)
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
                return Redirect("~/Account/Login");

            }
            else
            {
                var errorResponse = await responese.Content.ReadAsStringAsync();
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

                bool checkRoleAdmin = false;
                var checkRoles = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                if (checkRoles.StartsWith("Adm"))
                {
                    checkRoleAdmin = true;
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
                    claims.Add(new Claim("Name",Name));
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                Response.Cookies.Append("AccessToken", loginResult.AccessToken);
                if (checkRoleAdmin == true)
                {
                    _notyf.Success($"Login success! Welcome {obj.Email}");
                    return Redirect("~/Admin/Admin/Index");

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
            ViewBag.Roles = GetListRole();
            var lst = getapi.GetApi("Account");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Account obj, [Bind] IFormFile imageFile)
        {
            try
            {
               
                obj.Avatar= AddImg(imageFile);
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
            
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            return Redirect("~/Account/Login");
        }

        [HttpGet]
        public async Task<IActionResult> MyProfile(Guid id_User)
        {

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
                    Id_Role= user.IdRole,
                    

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
        [HttpPost]
        public async Task<IActionResult> MyProfile(AccountVm obj, [Bind] IFormFile imageFile)
        {
            try
            {
                var user = new Account()
                {
                    Id = obj.AccountId,
                    Email = obj.Email,
                    Name = obj.Name,
                    Password = obj.Password,
                    Avatar = AddImg(imageFile),
                    IdRole=obj.Id_Role

                };
                if (imageFile != null)
                {

                    user.Avatar = AddImg(imageFile);
                }
                else
                {

                    user.Avatar = "UserDefault.jpg";
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

                return Redirect("~/Home/Index");
            }
            catch
            {
                return View();
            }
        }
        public  List<Role> GetListRole()
        {
            var obj = _getapiRole.GetApi("Role");
            return obj;
        }
    }
}
