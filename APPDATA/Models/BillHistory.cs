using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class BillHistory
    {
        public Guid id { get; set; }
        public Guid? BIllId { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateBy { get; set; }
        public DateTime UpdateBy { get; set; }
        public Bill? Bill { get; set; }
    }
}
