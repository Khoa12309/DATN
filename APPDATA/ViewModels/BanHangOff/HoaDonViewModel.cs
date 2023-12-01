using APPDATA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.ViewModels.BanHangOff
{
    public class HoaDonViewModel
    {
        public List<BillDetail> ChiTietHoaDons { get; set; }
        public string Ten { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public string DiaChi { get; set; }
        public int TienShip { get; set; }
        public int TongTien { get; set; }
        public Guid? IDNhanVien { get; set; }
        public string? TenVoucher { get; set; }
        public Guid? IDNguoiDung { get; set; }
        public int? Diem { get; set; }
        public DateTime? NgayThanhToan { get; set; }
    }
}
