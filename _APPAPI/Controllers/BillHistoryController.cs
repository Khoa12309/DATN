using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillHistoryController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<BillHistory> _crud;
        public BillHistoryController()
        {
            _crud = new CRUDapi<BillHistory>(_context, _context.BillHistories);
        }
        [HttpGet]
        public IEnumerable<BillHistory> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(BillHistory obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            BillHistory item = _crud.GetAllItems().FirstOrDefault(c => c.id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(BillHistory obj)
        {
            BillHistory item = _crud.GetAllItems().FirstOrDefault(c => c.id == obj.id);

            item.BIllId = obj.BIllId;
            item.Description = obj.Description;
            item.CreateDate = obj.CreateDate;
            item.UpdateDate = obj.UpdateDate;
            item.CreateBy = obj.CreateBy;
            item.UpdateBy = obj.UpdateBy;


            return _crud.UpdateItem(item);
        }
    }
}
