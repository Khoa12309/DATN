using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class PaymentMethodDetail
    {
        public Guid id { get; set; }
        public Guid? BillId { get; set; }
        public Guid? PaymentMethodID { get; set; }

        public string TotalMoney { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public Bill? Bill  { get; set; }
        public PaymentMethod? PaymentMethods  { get; set; }
    }
}
