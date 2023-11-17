using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using _APPAPI.ViewModels;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
       
        // Phương thức GET để trả về danh sách các quận/huyện
        [HttpGet("districts")]
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            // Tạo một đối tượng OnlineGatewayClient để gọi API
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");

            // Gọi API [10] để lấy danh sách các quận/huyện theo tỉnh/thành phố
            var response = await client.GetDistrictsAsync();

            // Kiểm tra kết quả trả về
            //if (response.Code == 1) // Thành công
            //{
            //    // Trả về danh sách các quận/huyện dưới dạng JSON
            //    return Ok(response.Data);
            //}
            //else // Thất bại
            //{
            //    // Trả về thông báo lỗi
            //    return BadRequest(response.Message);
            //}
            return BadRequest("adsads");
        }

      
    }
}
