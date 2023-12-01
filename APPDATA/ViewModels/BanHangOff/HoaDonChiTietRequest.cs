using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.ViewModels.BanHangOff
{
    public class HoaDonChiTietRequest // Tạo chi tiết hóa đơn
    {
        public Guid Id { get; set; }
        public Guid IdHoaDon { get; set; }
        public Guid IdChiTietSanPham { get; set; }
        public int SoLuong { get; set; }
        //public int DonGia { get; set; } thanh toán r mới lưu
        public int TrangThai { get; set; }
    }
}
