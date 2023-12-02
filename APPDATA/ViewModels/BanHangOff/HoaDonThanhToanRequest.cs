using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.ViewModels.BanHangOff
{
    public class HoaDonThanhToanRequest
    {
        public Guid Id { get; set; }
        public Guid IdNhanVien { get; set; }
        public string PTTT { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public Guid IdVoucher { get; set; }
        public int TongTien { get; set; } // Khách phải trả
        public int TrangThai { get; set; }
        public string? GhiChu { get; set; }
    }
}
