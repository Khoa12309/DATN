using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartDetailsController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<CartDetail> _crud;
        public CartDetailsController()
        {
            _crud = new CRUDapi<CartDetail>(_context, _context.CartDetails);
        }
        [HttpGet]
        public IEnumerable<CartDetail> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(CartDetail obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            CartDetail item = _crud.GetAllItems().FirstOrDefault(c => c.id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(CartDetail obj)
        {
            CartDetail item = _crud.GetAllItems().FirstOrDefault(c => c.id == obj.id);

            item.ProductDetails = obj.ProductDetails;
            item.Price = obj.Price;
            item.Quantity = obj.Quantity;


            return _crud.UpdateItem(item);
        }
    }
}
