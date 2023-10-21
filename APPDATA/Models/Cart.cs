using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class Cart
    {
        public Guid id { get; set; }
        public Guid? AccountId { get; set; }
        public bool Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public List<CartDetail>? CartDetails { get; set; }
        public Account? Account { get; set; }
    }
}
