using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillDetailController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<BillDetail> _crud;
        public BillDetailController()
        {
            _crud = new CRUDapi<BillDetail>(_context, _context.BillDetails);
        }
        [HttpGet]

        public IEnumerable<BillDetail> GetAll()
        {
            return _crud.GetAllItems();
        }
        [Route("Post")]
        [HttpPost]
        public bool Creat(BillDetail obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            BillDetail item = _crud.GetAllItems().FirstOrDefault(c => c.id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(BillDetail obj)
        {
            BillDetail item = _crud.GetAllItems().FirstOrDefault(c => c.id == obj.id);
            item.Amount = obj.Amount;
            item.Price = obj.Price;
            item.Status = obj.Status;
            item.Price = obj.Price;
            item.Discount = obj.Discount;
            return _crud.UpdateItem(item);
        }
    }
}
