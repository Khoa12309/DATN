using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Address> _crud;
        public AddressController()
        {
            _crud = new CRUDapi<Address>(_context, _context.Address);
        }
        [HttpGet]
        public IEnumerable<Address> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(Address obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Address item = _crud.GetAllItems().FirstOrDefault(c => c.id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Address obj)
        {
            Address item = _crud.GetAllItems().FirstOrDefault(c => c.id == obj.id);

            item.Ward=  obj.Ward;
            item.PhoneNumber = obj.PhoneNumber;
            item.DefaultAddress = obj.DefaultAddress;
            item.Province = obj.Province;
            item.District = obj.District;
            item.Description = obj.Description;
            item.SpecificAddress = obj.SpecificAddress;
            item.City=obj.City;
            item.Name = obj.Name;
            return _crud.UpdateItem(item);
        }
    }
}
