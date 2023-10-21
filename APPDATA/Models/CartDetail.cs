using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class CartDetail
    {
        public Guid id { get; set; }
        public Guid? CartId { get; set; }
        public Guid? ProductDetail { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public Cart? Cart { get; set; }
        public ProductDetail? ProductDetails { get; set; }
    }
}
