using System.Net.Http.Headers;

namespace _APPAPI.ViewModels
{
    public class OnlineGateway
    {
       
        public void OnlineGatewayClient(string baseUrl, string userToken)
        {
            // Khởi tạo đối tượng HttpClient
          var   _client = new HttpClient();

            // Thiết lập header Authorization
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

            // Thiết lập base url
            _client.BaseAddress = new Uri(baseUrl);
        }

    }
}
