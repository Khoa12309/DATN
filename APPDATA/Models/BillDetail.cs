using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class BillDetail
    {
        public Guid id { get; set; }
        public Guid? ProductDetailID { get; set; }
        public Guid? BIllId { get; set; }
        public int Amount { get; set; }
        public float Price { get; set; }
        public float Discount { get; set; }
        public int Status { get; set; }
        public Bill? Bill { get; set; }
        public ProductDetail? ProductDetails { get; set; }
    }
}
