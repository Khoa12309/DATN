using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Category> _crud;
        public CategoryController()
        {
            _crud = new CRUDapi<Category>(_context, _context.Categories);
        }
        [HttpGet]
        public IEnumerable<Category> GetAll()
        {
            return _crud.GetAllItems().ToList();
        }
        [HttpPost]
        public bool Creat(Category obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Category item = _crud.GetAllItems().FirstOrDefault(c => c.Id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPost]
        public bool Update(Category obj)
        {
            Category item = _crud.GetAllItems().FirstOrDefault(c => c.Id == obj.Id);
            item.Name = obj.Name;
            item.Status = obj.Status;
            item.Update_date = obj.Update_date;
            item.Create_date = obj.Create_date;
            return _crud.UpdateItem(item);
        }
    }
}
