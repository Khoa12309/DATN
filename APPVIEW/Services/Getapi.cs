using Newtonsoft.Json;
using System.Collections;
using System.Text;

namespace APPVIEW.Services
{
    public class Getapi<T> where T : class
    {
        public  List<T> GetApi(string data)
        {
            var url = $"https://localhost:7042/api/";
            var httpClient = new HttpClient();
            var respones = httpClient.GetAsync(url + data).Result;

            var dataapi = respones.Content.ReadAsStringAsync().Result;
            var dataobj = JsonConvert.DeserializeObject<List<T>>(dataapi);
            return dataobj;
        }
        public async Task<T> CreateObj(T obj, string name)
        {

            string data = JsonConvert.SerializeObject(obj);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            string requestURL =
            $"https://localhost:7042/api/";
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(requestURL + name + "/Post", content);
            string apiData = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                
                return obj;
            }
            return obj;
        }
        public async Task<T> UpdateObj(T obj, string name)
        {
            string data = JsonConvert.SerializeObject(obj);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            string requestURL =
            $"https://localhost:7042/api/";
            var httpClient = new HttpClient(); // Tại 1 httpClient để call API
            var response = await httpClient.PutAsync(requestURL + name + "/Update", content);

            string apiData = await response.Content.ReadAsStringAsync();
            return obj;       
        }

        public async Task<bool> DeleteObj(Guid id, string name)
        {

            string requestURL =
            $"https://localhost:7042/api/";
            var httpClient = new HttpClient(); // Tại 1 httpClient để call API
            var response = await httpClient.DeleteAsync(requestURL + name + "/Delete?id=" + id.ToString());

            string apiData = await response.Content.ReadAsStringAsync();
            return true;

        }
    }
}
