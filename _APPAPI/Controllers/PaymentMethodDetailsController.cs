using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodDetailsController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<PaymentMethodDetail> _crud;
        public PaymentMethodDetailsController()
        {
            _crud = new CRUDapi<PaymentMethodDetail>(_context, _context.PaymentMethodDetails);
        }
        [HttpGet]
        public IEnumerable<PaymentMethodDetail> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(PaymentMethodDetail obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            PaymentMethodDetail item = _crud.GetAllItems().FirstOrDefault(c => c.id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(PaymentMethodDetail obj)
        {
            PaymentMethodDetail item = _crud.GetAllItems().FirstOrDefault(c => c.id == obj.id);

            item.PaymentMethodID = obj.PaymentMethodID;
            item.TotalMoney = obj.TotalMoney;
            item.Status = obj.Status;
            item.BillId = obj.BillId;
            item.Description = obj.Description;
           


            return _crud.UpdateItem(item);
        }
    }
}
