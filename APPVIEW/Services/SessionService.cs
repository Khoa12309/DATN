using APPDATA.Models;
using Newtonsoft.Json;

namespace _APPAPI.Service
{
    public class SessionService
    {
        public static void SetObjToJson(ISession session, string key, object value)
        {

            var jsonString = JsonConvert.SerializeObject(value);

            session.SetString(key, jsonString);

        }
        public static List<ProductDetail> GetObjFromSession(ISession session, string key)
        {

            var data = session.GetString(key);
            if (data != null)
            {
                var listObj = JsonConvert.DeserializeObject<List<ProductDetail>>(data);
                return listObj;
            }
            else return new List<ProductDetail>();
        }
        public static Account GetUserFromSession(ISession session, string key)
        {
            var data = session.GetString(key);
            if (data != null)
            {
                var listObj = JsonConvert.DeserializeObject<Account>(data);
                return listObj;
            }
            else return new Account();
        }
        public static bool CheckProductInCart(Guid id, List<ProductDetail> cartProducts)
        {
            return cartProducts.Any(p => p.Id == id);
        }
    }
}
