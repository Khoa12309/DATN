using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Product> _crud;
        public ProductController()
        {
            _crud = new CRUDapi<Product>(_context, _context.Products);
        }
        [HttpGet]
        public IEnumerable<Product> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(Product obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Product item = _crud.GetAllItems().FirstOrDefault(c => c.Id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Product obj)
        {
            Product item = _crud.GetAllItems().FirstOrDefault(c => c.Id == obj.Id);
         
            item.Update_date=obj.Update_date;
            item.Status=obj.Status;
            item.Code=obj.Code;
            item.Name=obj.Name;
            

            return _crud.UpdateItem(item);
        }
    }
}
