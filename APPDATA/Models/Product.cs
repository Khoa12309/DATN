using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class Product
    {
        public Guid Id { get; set; } 
        public Guid? Id_supplier { get; set; } 
        public string Name { get; set; }
        public string Code { get; set; } 
        public int Status { get; set; } 
        public DateTime Create_date  { get; set; }
        public DateTime Update_date  { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
