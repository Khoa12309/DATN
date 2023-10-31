using _APPAPI.Service;
using APPDATA.DB;
using APPDATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        ShoppingDB _context = new ShoppingDB();
        private readonly CRUDapi<Image> _crud;
        public ImageController()
        {
            _crud = new CRUDapi<Image>(_context, _context.Images);
        }
        [HttpGet]
        public IEnumerable<Image> Getall()
        {
            return _crud.GetAllItems().ToList();
        }
        [Route("Post")]
        [HttpPost]
        public bool Create(Image obj)
        {
            return _crud.CreateItem(obj);
        }
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(Guid id)
        {
            Image item = _crud.GetAllItems().FirstOrDefault(c => c.Id == id);
            return _crud.DeleteItem(item);
        }
        [Route("Update")]
        [HttpPut]
        public bool Update(Image obj)
        {
            Image item = _crud.GetAllItems().FirstOrDefault(c => c.Id == obj.Id);

            item.Update_date = DateTime.Now;
            item.Status = obj.Status;
            item.IdProductdetail = obj.IdProductdetail;
            item.Name = obj.Name;
            return _crud.UpdateItem(item);
        }

    }
}
