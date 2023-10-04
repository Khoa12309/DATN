using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class Size
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public DateTime Create_date { get; set; }
        public DateTime Update_date { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }

    }
}
