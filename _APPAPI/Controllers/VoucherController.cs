using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Voucher> _crud;
        public VoucherController()
        {
            _crud = new CRUDapi<Voucher>(_context, _context.Vouchers);
        }
        [HttpGet]
        public IEnumerable<Voucher> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(Voucher obj)
        {
            obj.Create_date = DateTime.Now;
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Voucher item = _crud.GetAllItems().FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                // Kiểm tra xem voucher đã được sử dụng hay chưa
                if (IsVoucherUsed(item) || VoucherHasBeenSaved(item))
                {
                    // Nếu đã sử dụng, thay đổi trạng thái thành 2
                    item.Status = 2;
                    item.Update_date = DateTime.Now;
                    UpdateVoucherForAccStatus(item.Id, item.Status);
                    return _crud.UpdateItem(item);
                }
                else
                {
                    // Nếu chưa sử dụng, xóa voucher khỏi cơ sở dữ liệu
                    return _crud.DeleteItem(item);
                }
                
            }
            return false;
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Voucher obj)
        {
            Voucher item = _crud.GetAllItems().FirstOrDefault(c => c.Id == obj.Id);

            item.Value = obj.Value;
            item.Name = obj.Name;
            item.Code = obj.Code;
            item.Quantity = obj.Quantity;
            item.Status = obj.Status;
            item.DiscountAmount = obj.DiscountAmount;
            item.Update_date = DateTime.Now;
            item.StartDate = obj.StartDate;
            item.EndDate = obj.EndDate;
            UpdateVoucherForAccStatus(item.Id, obj.Status);

            return _crud.UpdateItem(item);
        }
        private bool IsVoucherUsed(Voucher voucher)
        {
            // Kiểm tra trong bảng đơn hàng
            var billsWithVoucher = _context.Bills.Where(bill => bill.Voucherid == voucher.Id);

            return billsWithVoucher.Any();
        }
        private bool VoucherHasBeenSaved(Voucher voucher)
        {
            // Kiểm tra trong tài khoản
            var accWithVoucher = _context.VoucherForAccs.Where(acc => acc.Id_Voucher == voucher.Id);

            return accWithVoucher.Any();
        }
        private void UpdateVoucherForAccStatus(Guid voucherId, int status)
        {
            var voucherForAccs = _context.VoucherForAccs.Where(va => va.Id_Voucher == voucherId).ToList();

            foreach (var voucherForAcc in voucherForAccs)
            {
                voucherForAcc.Status = status;
            }

            _context.SaveChanges();
        }
    }
}
