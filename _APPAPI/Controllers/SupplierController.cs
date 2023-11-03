using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {

        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Supplier> _crud;
        public SupplierController()
        {
            _crud = new CRUDapi<Supplier>(_context, _context.Suppliers);
        }
        [HttpGet]
        public IEnumerable<Supplier> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(Supplier obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Supplier item = _crud.GetAllItems().FirstOrDefault(c => c.Id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Supplier obj)
        {
            Supplier item = _crud.GetAllItems().FirstOrDefault(c => c.Id == obj.Id);
            item.Suppliercode = obj.Suppliercode;
            item.PhoneNumber = obj.PhoneNumber;
            item.Address = obj.Address;
            item.Desciption = obj.Desciption;

            item.Name = obj.Name;


            return _crud.UpdateItem(item);
        }
    }
}
