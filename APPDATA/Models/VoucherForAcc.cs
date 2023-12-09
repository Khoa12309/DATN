using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class VoucherForAcc
    {
        [Key]
        public Guid Id { get; set; }
        public int Status { get; set; }
        [ForeignKey("Account")]
        public Guid Id_Account { get; set; }
        [ForeignKey("Voucher")]
        public Guid Id_Voucher { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public double DiscountAmount { get; set; }
        public DateTime EndDate { get; set; }
        public virtual Voucher? Voucher { get; set; }
        public virtual Account? Account { get; set; }
        public List<Bill>? Bill { get; set; }
    }
}
