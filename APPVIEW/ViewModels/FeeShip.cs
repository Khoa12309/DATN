

namespace APPVIEW.ViewModels
{
    public class FeeShip
    {
        public int Code { get; set; }

        // Thuộc tính Message
       
        public string Message { get; set; }
        public phi Data { get; set; }

        // Thuộc tính Data
       
       public class phi
        {
            public int insurance_fee { get; set; }
            public int total { get; set; }
        }
      
        public int pick_station_fee { get; set; }
        public int coupon_value { get; set; }
        public int r2s_fee { get; set; }
        public int return_again { get; set; }
        public int document_return { get; set; }
        public int double_check { get; set; }
        public int cod_fee { get; set; }
        public int pick_remote_areas_fee { get; set; }
        public int deliver_remote_areas_fee { get; set; }
        public int cod_failed_fee { get; set; }
      
       
        
        
    }
}
