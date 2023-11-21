using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Material> _crud;
        public MaterialController()
        {
            _crud = new CRUDapi<Material>(_context, _context.Materials);
        }
        [HttpGet]
        public IEnumerable<Material> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(Material obj)
        {
            obj.Create_date = DateTime.Now;
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Material item = _crud.GetAllItems().FirstOrDefault(c => c.Id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Material obj)
        {
            Material item = _crud.GetAllItems().FirstOrDefault(c => c.Id == obj.Id);

            item.Update_date = DateTime.Now;
            item.Status = obj.Status;
            item.Name = obj.Name;
            return _crud.UpdateItem(item);
        }
    }
}
