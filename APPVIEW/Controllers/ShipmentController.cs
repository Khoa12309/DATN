using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _APPAPI.ViewModels;
using Newtonsoft.Json;

namespace _APPAPI.Controllers
{
    
    public class ShipmentController : ControllerBase
    {
       
        // Phương thức GET để trả về danh sách các quận/huyện
        [HttpGet("province")]
        public async Task<object> GetProvince()
        {
            // Tạo một đối tượng OnlineGatewayClient để gọi API
            var client = new OnlineGatewayClient($"https://online-gateway.ghn.vn/shiip/public-api/master-data/province", "bdbbde2a-fec2-11ed-8a8c-6e4795e6d902");

            // Gọi API [10] để lấy danh sách các tỉnh/thành phố
            var response = await client.GetProvincesAsync();
            var lis = new List<object>();
            //Kiểm tra kết quả trả về
            if (response.Code == 200) // Thành công
            {
                // Trả về danh sách các quận/huyện dưới dạng JSON
                var data = response.Data;
                
                // Duyệt qua danh sách các đối tượng Province trong data
                //foreach (var province in data)
                //{
                  

                //    // In ra thông tin của mỗi đối tượng Province
                //    lis.Add(province);
                //}
                return response;
            }
            else // Thất bại
            {
                // Trả về thông báo lỗi
                return BadRequest(response.Message);
            }
          
        }

      
    }
}
