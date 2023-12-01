using APPDATA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPDATA.ViewModels.BanHangOff
{
    public class ChiTietSanPhamBanHang
    {
        public Guid Id { get; set; }
        public string Ten { get; set; }
        public List<Color> lstMau { get; set; }
        public List<Size> lstKC { get; set; }
    }
}
