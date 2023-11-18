using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<PaymentMethod> _crud;
        public PaymentMethodController()
        {
            _crud = new CRUDapi<PaymentMethod>(_context, _context.PaymentMethods);
        }
        [HttpGet]
        public IEnumerable<PaymentMethod> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(PaymentMethod obj)
        {
            obj.CreateDate = DateTime.Now;
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            PaymentMethod item = _crud.GetAllItems().FirstOrDefault(c => c.id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(PaymentMethod obj)
        {
            PaymentMethod item = _crud.GetAllItems().FirstOrDefault(c => c.id == obj.id);

            item.UpdateDate = DateTime.Now;
            item.UpdateBy = obj.UpdateBy;
            item.Status = obj.Status;
            item.CreateBy = obj.CreateBy;
            item.Description = obj.Description;
            item.Method = obj.Method;
            
           


            return _crud.UpdateItem(item);
        }
    }
}
