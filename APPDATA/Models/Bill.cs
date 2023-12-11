using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class Bill
    {
        public Guid id { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? Voucherid { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public float? ShipFee { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public float? TotalMoney { get; set; }
        public float? MoneyReduce { get; set; }
        public string Type { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? CreateBy { get; set; }
        public DateTime? UpdateBy { get; set; }
        public DateTime? PayDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public int Status { get; set; }
        public List<BillDetail>? BillDetails { get; set; }
        public List<PaymentMethodDetail>? PaymentMethodDetails { get; set; }
        public Account? Account { get; set; }
        public Voucher? Voucher { get; set; }
        public List<BillHistory>? BillHistory { get; set; }

    }
}
