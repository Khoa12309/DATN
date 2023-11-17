using _APPAPI.ViewModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace _APPAPI.Controllers
{
    public class OnlineGatewayClient
    {
        HttpClient _client;
        public OnlineGatewayClient(string baseUrl, string userToken)
        {

            // Khởi tạo đối tượng HttpClient
           _client = new HttpClient();
           _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
         

           // Thiết lập header Authorization
          _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", userToken);

            // Thiết lập base url
            _client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<List<Province>> GetDistrictsAsync()
        {
            // Tạo một đối tượng HttpRequestMessage
            var request = new HttpRequestMessage();

            // Thiết lập phương thức HTTP là GET
            request.Method = HttpMethod.Get;

            // Thiết lập đường dẫn URL và tham số query
           // request.RequestUri = new Uri($"?province_id={13}");
            // Gửi yêu cầu HTTP
            var response = await _client.SendAsync(request);

            // Kiểm tra trạng thái của kết quả trả về
            if (response.IsSuccessStatusCode) // Thành công
            {
                // Đọc nội dung của kết quả trả về
                var content = await response.Content.ReadAsStringAsync();

                // Chuyển đổi nội dung từ JSON sang đối tượng OnlineGatewayResponse
                var result = JsonConvert.DeserializeObject<List<Province>>(content);

                // Trả về đối tượng OnlineGatewayResponse
                return result;
            }
            else // Thất bại
            {
                // Ném ra một ngoại lệ
                response.EnsureSuccessStatusCode();
            }
            // Các bước từ 2 đến 4
            return new List<Province>();
        }
    }

}