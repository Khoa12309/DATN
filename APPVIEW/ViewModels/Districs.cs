namespace APPVIEW.ViewModels
{
    public class Districs
    {
        public int ProvinceID { get; set; }
        public int DistrictID { get; set; }
        public string DistrictName { get; set; }
        public string Code { get; set; }
        public string[] NameExtension { get; set; }

        //"DistrictID": 2027,
        //   "ProvinceID": 232,
        //   "DistrictName": "Huyện Thanh Liêm",
        //   "Code": "2405",
        //   "Type": 2,
        //   "SupportType": 3,
        //   "NameExtension": [
        //       "Huyện Thanh Liêm",
        //       "H.Thanh Liêm",
        //       "H Thanh Liêm",
        //       "Thanh Liêm",
        //       "Thanh Liem",
        //       "Huyen Thanh Liem",
        //       "thanhliem"
        //   ],
    }
}
