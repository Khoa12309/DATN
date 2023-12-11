using _APPAPI.Service;
using _APPAPI.ViewModels;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Account> _crud;

        private readonly IConfiguration _Configuration;
        public AccountController(IConfiguration configuration)
        {
            _crud = new CRUDapi<Account>(_context, _context.Accounts);
            _Configuration = configuration;
        }
        [HttpGet]
        public IEnumerable<Account> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool addaccemty(Account obj)
        {          
            return _crud.CreateItem(obj);
        }
        [Route("Register")]
        [HttpPost]
        public bool Create(Account obj)
        {
            obj.Create_date = DateTime.Now;
            obj.Status = 1;
            var checkRole = _context.Roles.Count();

            if (checkRole == 0)
            {
                var customerRole = new Role()
                {
                    id = Guid.NewGuid(),
                    name = "Customer",
                    Status = 1
                };
                _context.Roles.Add(customerRole);
               
                var adminRole = new Role()
                {
                    id = Guid.NewGuid(),
                    name = "Admin",
                    Status = 1
                };
                _context.Roles.Add(adminRole);
               
                var staffRole = new Role()
                {
                    id = Guid.NewGuid(),
                    name = "Staff",
                    Status = 1
                };
                _context.Roles.Add(staffRole);
                _context.SaveChangesAsync();
            }
            if (obj.IdRole == null)
            {
                obj.IdRole = _context.Roles.SingleOrDefault(c => c.name == "Customer").id;
                obj.Status = 0;
            }

            obj.Avatar = "UserDefault.jpg";

            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpPost]
        public bool Delete(Guid id)
        {
            Account item = _crud.GetAllItems().FirstOrDefault(c => c.Id == id);
            item.Status = 0;
            return _crud.UpdateItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Account obj)
        {
            Account item = _crud.GetAllItems().FirstOrDefault(c => c.Id == obj.Id);

            item.Update_date = DateTime.Now;
            item.Avatar = obj.Avatar;
            item.Email = obj.Email;
            item.Password = obj.Password;
            item.IdRole = obj.IdRole;
            item.Status = obj.Status;
            item.Name = obj.Name;
            return _crud.UpdateItem(item);
        }
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Validate(LoginVm model)
        {
            var user = _context.Accounts.SingleOrDefault(p => p.Email == model.Email && model.Password == p.Password);
            if (user == null||user.ResetPasswordcode!=null) //không đúng
            {
                return BadRequest();
            }


            //cấp token


            var token = await GenerateToken(user);
            return Ok(new TokenVm
            {
                AccessToken =
                token.AccessToken,
                RefreshToken = token.RefreshToken
            });

        }
        private async Task<TokenVm> GenerateToken(Account item)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_Configuration["JWT:SecretKey"]);

            var role = _context.Roles.SingleOrDefault(p => p.id == item.IdRole);


            var tokenDescription = new SecurityTokenDescriptor
            {


                Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, item.Name),
                new Claim(JwtRegisteredClaimNames.Email, item.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", item.Id.ToString()),
                new Claim("Id_Role", item.IdRole.ToString()),
                new Claim("Avatar", item.Avatar.ToString()),
                new Claim("Name", item.Name.ToString()),
                new Claim(ClaimTypes.Role,role.name),
              


                //roles
            }),


                Expires = DateTime.UtcNow.AddMinutes(20),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)

            };



            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();
            //Lưu database
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                UserId = item.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(1)
            };
            await _context.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenVm
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenVm model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_Configuration["JWT:SecretKey"]);
            var tokenValidateParam = new TokenValidationParameters
            {
                //tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,

                //ký vào token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ClockSkew = TimeSpan.Zero,

                ValidateLifetime = false //ko kiểm tra token hết hạn
            };
            try
            {
                //check 1: AccessToken valid format
                var tokenInVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidateParam, out var validatedToken);

                //check 2: Check alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)//false
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Invalid token"
                        });
                    }
                }

                //check 3: Check accessToken expire?
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Access token has not yet expired"
                    });
                }

                //check 4: Check refreshtoken exist in DB
                var storedToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == model.RefreshToken);
                if (storedToken == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token does not exist"
                    });
                }

                //check 5: check refreshToken is used/revoked?
                if (storedToken.IsUsed)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token has been used"
                    });
                }
                if (storedToken.IsRevoked)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token has been revoked"
                    });
                }

                //check 6: AccessToken id == JwtId in RefreshToken
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Token doesn't match"
                    });
                }

                //Update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                _context.Update(storedToken);
                await _context.SaveChangesAsync();

                //create new token
                var user = await _context.Accounts.SingleOrDefaultAsync(nd => nd.Id == storedToken.UserId);
                var token = await GenerateToken(user);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Renew token success",
                    Data = token
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Something went wrong"
                });
            }
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
        }
    }
}
