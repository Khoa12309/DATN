using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Bill> _crud;
        public BillController()
        {
            _crud = new CRUDapi<Bill>(_context, _context.Bills);
        }
        [HttpGet]
        public IEnumerable<Bill> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(Bill obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Bill item = _crud.GetAllItems().FirstOrDefault(c => c.id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Bill obj)
        {
            Bill item = _crud.GetAllItems().FirstOrDefault(c => c.id == obj.id);

           // item.AccountId = obj.AccountId;
          //  item.Voucherid = obj.Voucherid;
            item.Code = obj.Code;
            item.ShipFee = obj.ShipFee;
            item.PhoneNumber = obj.PhoneNumber;
            item.Address= obj.Address;  
            item.TotalMoney = obj.TotalMoney;
            item.MoneyReduce = obj.MoneyReduce;
            item.Type = obj.Type;
            item.Status = obj.Status;
            item.CreateBy = obj.CreateBy;
            item.CreateDate = obj.CreateDate;
            item.PayDate = obj.PayDate;
            
            return _crud.UpdateItem(item);
        }
    }
}
