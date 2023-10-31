using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Account> _crud;
        public AccountController()
        {
            _crud = new CRUDapi<Account>(_context, _context.Accounts);
        }
        [HttpGet]
        public IEnumerable<Account> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(Account obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Account item = _crud.GetAllItems().FirstOrDefault(c => c.Id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Account obj)
        {
            Account item = _crud.GetAllItems().FirstOrDefault(c => c.Id == obj.Id);

            item.Update_date = DateTime.Now;
            item.Avatar = obj.Avatar;
            item.Email = obj.Email;
            item.Password = obj.Password;
            item.IdRole = obj.IdRole;
            
            item.Name = obj.Name;
            return _crud.UpdateItem(item);
        }
    }
}
