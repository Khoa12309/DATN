using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetailsController : ControllerBase
    {
        
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<ProductDetail> _crud;
        public ProductDetailsController()
        {
            _crud = new CRUDapi<ProductDetail>(_context, _context.ProductDetails);
        }
        [HttpGet]
        public IEnumerable<ProductDetail> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(ProductDetail obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            ProductDetail item = _crud.GetAllItems().FirstOrDefault(c => c.Id == id);
            if (!_crud.DeleteItem(item))
            {
                item.Status = 0 ;
                _crud.UpdateItem(item);
            } 
            return true;
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(ProductDetail obj)
        {
            ProductDetail item = _crud.GetAllItems().FirstOrDefault(c => c.Id == obj.Id);

            item.Update_date = obj.Update_date;
            item.Status = obj.Status;
            item.Update_by = obj.Update_by;
            item.Id_Category = obj.Id_Category;
            item.Desciption = obj.Desciption;
            item.Id_Color = obj.Id_Color;
            item.Id_Material = obj.Id_Material;
            item.Price = obj.Price;
            item.Quantity = obj.Quantity;
            item.Id_Size = obj.Id_Size;
            item.Id_supplier = obj.Id_supplier;
            item.Name = obj.Name;
            item.Id_Product = obj.Id_Product;
            return _crud.UpdateItem(item);
        }

    }
}
