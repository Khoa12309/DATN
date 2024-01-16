using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.Models
{
    public class Voucher
    {
        public Guid Id { get; set; }
        public Guid? AccountId { get; set; }
        [Range(1,100, ErrorMessage = "Giá trị % phải lớn hơn 0 và nhỏ hơn 50")]
        public int Value { get; set; }
        [Required(ErrorMessage = "Tên là bắt buộc")]
        public string Name { get; set; }
        public string Code { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
        public int Status { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Giảm tối đa không được âm")]
        public double DiscountAmount { get; set; }
        public DateTime Create_date { get; set; }
        public DateTime Update_date { get; set; }
        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime EndDate { get; set; }
        public List<Account>? Accounts { get; set; }
        public List<Bill>? Bill { get; set; }
    }
}
