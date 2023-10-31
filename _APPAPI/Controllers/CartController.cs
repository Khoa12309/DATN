using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Cart> _crud;
        public CartController()
        {
            _crud = new CRUDapi<Cart>(_context, _context.Carts);
        }
        [HttpGet]
        public IEnumerable<Cart> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(Cart obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Cart item = _crud.GetAllItems().FirstOrDefault(c => c.id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Cart obj)
        {
            Cart item = _crud.GetAllItems().FirstOrDefault(c => c.id == obj.id);

            item.Status = obj.Status;
            item.CreateDate = obj.CreateDate;
            item.UpdateDate = obj.UpdateDate;

        


            return _crud.UpdateItem(item);
        }
    }
}
