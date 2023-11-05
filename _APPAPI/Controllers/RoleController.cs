using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Role> _crud;
        public RoleController()
        {
            _crud = new CRUDapi<Role>(_context, _context.Roles);
        }
        [HttpGet]
        public IEnumerable<Role> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(Role obj)
        {
           
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Role item = _crud.GetAllItems().FirstOrDefault(c => c.id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Role obj)
        {
            Role item = _crud.GetAllItems().FirstOrDefault(c => c.id == obj.id);

            item.name = obj.name;
            item.Status = obj.Status;

            return _crud.UpdateItem(item);
        }
    }
}

