using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizeController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Size> _crud;
        public SizeController()
        {
            _crud = new CRUDapi<Size>(_context, _context.Sizes);
        }
        [HttpGet]
        public IEnumerable<Size> GetAll()
        {
            return _crud.GetAllItems().ToList();
        }

        [Route("Post")]
        [HttpPost]
        public bool Creat(Size obj)
        {
            obj.Create_date = DateTime.Now;
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Size item = _crud.GetAllItems().FirstOrDefault(c => c.Id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Size obj)
        {
            Size item = _crud.GetAllItems().FirstOrDefault(c => c.Id == obj.Id);
            item.Name = obj.Name;
            item.Status = obj.Status;
            item.Update_date = DateTime.Now;
            return _crud.UpdateItem(item);
        }
    }
}
