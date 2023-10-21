using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class Supplier
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Suppliercode { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Desciption { get; set; }
        public List<ProductDetail>? ProductDetails { get; set; }
    }
}
