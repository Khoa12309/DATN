using APPDATA.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System;
using System.Net.Http;

using System.Threading.Tasks;
using APPVIEW.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net.WebSockets;

namespace _APPAPI.ViewModels
{
    public class OnlineGatewayClient
    {
        private HttpClient _client;

        // Constructor nhận vào base url và user token
        public OnlineGatewayClient(string baseUrl, string userToken)
        {
            // Khởi tạo đối tượng HttpClient
            _client = new HttpClient();
           
            _client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml+json");
            _client.DefaultRequestHeaders.Add("token", userToken);
          
            // Thiết lập header Authorization
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

            // Thiết lập base url
            _client.BaseAddress = new Uri(baseUrl);
        }

        // Phương thức GetProvincesAsync để lấy danh sách các tỉnh/thành phố
        public async Task<OnlineGatewayResponse<Province>> GetProvincesAsync()
        {
            // Tạo một đối tượng HttpRequestMessage
            var request = new HttpRequestMessage();

            // Thiết lập phương thức HTTP là GET
            request.Method = HttpMethod.Get;
            
            // Thiết lập đường dẫn URL
            request.RequestUri = _client.BaseAddress;
           
           

            // Gửi yêu cầu HTTP
            var response = await _client.GetAsync(request.RequestUri);

            // Kiểm tra trạng thái của kết quả trả về
            if (response.IsSuccessStatusCode) // Thành công
            {
               
            }
            else // Thất bại
            {
                // Ném ra một ngoại lệ
                response.EnsureSuccessStatusCode();
            }
            // Đọc nội dung của kết quả trả về
            var content = await response.Content.ReadAsStringAsync();

            // Chuyển đổi nội dung từ JSON sang đối tượng OnlineGatewayResponse
            var result = JsonConvert.DeserializeObject<OnlineGatewayResponse<Province>>(content);

            // Trả về đối tượng OnlineGatewayResponse
            return result;
        }
      
       public async Task<FeeShip> GetFeeshipAsync()
        {

            var request = new HttpRequestMessage();

            request.Method = HttpMethod.Get;
            request.RequestUri = _client.BaseAddress;
            
            var response = await _client.GetAsync(request.RequestUri);
            var content = await response.Content.ReadAsStringAsync();   
            
            var result = JsonConvert.DeserializeObject<FeeShip>(content);
            
            return result ;

       }
        public async Task<OnlineGatewayResponse<Districs>> GetDistricsAsync()
        {
            // Tạo một đối tượng HttpRequestMessage
            var request = new HttpRequestMessage();

            // Thiết lập phương thức HTTP là GET
            request.Method = HttpMethod.Get;
            
            // Thiết lập đường dẫn URL
            request.RequestUri = _client.BaseAddress;
            // Gửi yêu cầu HTTP
            var response = await _client.GetAsync(request.RequestUri);

            // Kiểm tra trạng thái của kết quả trả về
            if (response.IsSuccessStatusCode) // Thành công
            {
               
            }
            else // Thất bại
            {
                // Ném ra một ngoại lệ
                response.EnsureSuccessStatusCode();
            }
            // Đọc nội dung của kết quả trả về
            var content = await response.Content.ReadAsStringAsync();

            // Chuyển đổi nội dung từ JSON sang đối tượng OnlineGatewayResponse
            var result = JsonConvert.DeserializeObject<OnlineGatewayResponse<Districs>>(content);

            // Trả về đối tượng OnlineGatewayResponse
            return result;
        } 
        public async Task<OnlineGatewayResponse<ShipService>> GetServiceAsync()
        {
            // Tạo một đối tượng HttpRequestMessage
            var request = new HttpRequestMessage();

            // Thiết lập phương thức HTTP là GET
            request.Method = HttpMethod.Get;
            
            // Thiết lập đường dẫn URL
            request.RequestUri = _client.BaseAddress;
            // Gửi yêu cầu HTTP
            var response = await _client.GetAsync(request.RequestUri);

            // Kiểm tra trạng thái của kết quả trả về
            if (response.IsSuccessStatusCode) // Thành công
            {
               
            }
            else // Thất bại
            {
                // Ném ra một ngoại lệ
                response.EnsureSuccessStatusCode();
            }
            // Đọc nội dung của kết quả trả về
            var content = await response.Content.ReadAsStringAsync();

            // Chuyển đổi nội dung từ JSON sang đối tượng OnlineGatewayResponse
            var result = JsonConvert.DeserializeObject<OnlineGatewayResponse<ShipService>>(content);

            // Trả về đối tượng OnlineGatewayResponse
            return result;
        } 
        public async Task<OnlineGatewayResponse<Ward>> GetWardsAsync()
        {
            // Tạo một đối tượng HttpRequestMessage
            var request = new HttpRequestMessage();

            // Thiết lập phương thức HTTP là GET
            request.Method = HttpMethod.Get;
            
            // Thiết lập đường dẫn URL
            request.RequestUri = _client.BaseAddress;
            // Gửi yêu cầu HTTP
            var response = await _client.GetAsync(request.RequestUri);

            // Kiểm tra trạng thái của kết quả trả về
            if (response.IsSuccessStatusCode) // Thành công
            {
               
            }
            else // Thất bại
            {
                // Ném ra một ngoại lệ
                response.EnsureSuccessStatusCode();
            }
         
            // Đọc nội dung của kết quả trả về
            var content = await response.Content.ReadAsStringAsync();

            // Chuyển đổi nội dung từ JSON sang đối tượng OnlineGatewayResponse
            var result = JsonConvert.DeserializeObject<OnlineGatewayResponse<Ward>>(content);

            // Trả về đối tượng OnlineGatewayResponse
            return result;
        }
    }


    }