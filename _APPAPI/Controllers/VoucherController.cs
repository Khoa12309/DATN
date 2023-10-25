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
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Voucher item = _crud.GetAllItems().FirstOrDefault(c => c.Id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Voucher obj)
        {
            Voucher item = _crud.GetAllItems().FirstOrDefault(c => c.Id == obj.Id);

            item.Value = obj.Value;
            item.Name = obj.Name;
            item.Code = obj.Code;
            item.ReduceForm = obj.ReduceForm;
            item.Status = obj.Status;
            item.DiscountAmount = obj.DiscountAmount;
            item.Create_date = obj.Create_date;
            item.Update_date = obj.Update_date;
            item.StartDate = obj.StartDate;
            item.EndDate = obj.EndDate;


            return _crud.UpdateItem(item);
        }
    }
}
