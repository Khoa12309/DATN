using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Color> _crud;
        public ColorController()
        {
            _crud = new CRUDapi<Color>(_context, _context.Colors);
        }
        [HttpGet]
        public IEnumerable<Color> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(Color obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Color item = _crud.GetAllItems().FirstOrDefault(c => c.Id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Color obj)
        {
            Color item = _crud.GetAllItems().FirstOrDefault(c => c.Id == obj.Id);

            item.Update_date = obj.Update_date;
            item.Status = obj.Status;
            item.Update_date=obj.Update_date;
            
            item.Name = obj.Name;


            return _crud.UpdateItem(item);
        }
    }
}
