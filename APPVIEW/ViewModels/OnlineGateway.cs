using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace _APPAPI.ViewModels
{
    public class OnlineGatewayResponse<TResult>
    {

        public OnlineGatewayResponse(int code, string message, TResult[] data)
        {
            // Gán code cho thuộc tính Code
            this.Code = code;

            // Gán message cho thuộc tính Message
            this.Message = message;

            // Gán data cho thuộc tính Data
            this.Data = data;
        }

        // Thuộc tính Code
        [JsonProperty("code")]
        public int Code { get; set; }

        // Thuộc tính Message
        [JsonProperty("message")]
        public string Message { get; set; }

        // Thuộc tính Data
        [JsonProperty("data")]
        public TResult[] Data { get; set; }
    }
}
