using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherForAccController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<VoucherForAcc> _crud;
        public VoucherForAccController()
        {
            _crud = new CRUDapi<VoucherForAcc>(_context, _context.VoucherForAccs);
        }
        [HttpGet]
        public IEnumerable<VoucherForAcc> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(VoucherForAcc obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            VoucherForAcc item = _crud.GetAllItems().FirstOrDefault(c => c.Id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(VoucherForAcc obj)
        {
            VoucherForAcc item = _crud.GetAllItems().FirstOrDefault(c => c.Id == obj.Id);

            item.Status = obj.Status;


            return _crud.UpdateItem(item);
        }
    }
}
