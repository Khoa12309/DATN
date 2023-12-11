using APPDATA.Models;
using APPVIEW.Services;
//using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace APPVIEW.Controllers
{
   // [Authorize(Roles = "Admin")]
    public class ImageController : Controller
    {
        private Getapi<Image> getapi;
        private Getapi<ProductDetail> getapipd;
        private Getapi<Product> getapip;
        public ImageController()
        {
            getapi = new Getapi<Image>();
            getapipd = new Getapi<ProductDetail>();
            getapip = new Getapi<Product>();
        }

        public async Task<IActionResult> GetList()
        {

            ViewBag.PD = await getapi.GetApia("ProductDetails");
            var obj = getapi.GetApi("Image");          
            return View( obj);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.PD = getapipd.GetApi("ProductDetails");
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Image obj, [Bind] List<IFormFile> imageFile)
        {
            try
            {
                foreach (var item in imageFile)
                {
                    if (imageFile != null && item.Length > 0) // Không null và không trống
                    {
                        //Trỏ tới thư mục wwwroot để lát nữa thực hiện việc Copy sang
                        var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot", "images", item.FileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            // Thực hiện copy ảnh vừa chọn sang thư mục mới (wwwroot)
                            item.CopyTo(stream);
                        }
                        // Gán lại giá trị cho Description của đối tượng bằng tên file ảnh đã được sao chép
                        obj.Name = item.FileName;
                    }
                    await getapi.CreateObj(obj, "Image");
                }
             
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }
        //public async Task<IActionResult> Search(string searchTerm)
        //{
        //    var lstProduct = getapi.GetApi("Imdge").ToList();
            

        //    var pd= getapipd.GetApi("ProductDetails").GroupBy(c=>c.Name.ToLower().Contains(searchTerm.ToLower()));
           


        //    var searchResult = lstProduct.GroupBy()
               
        //        .ToList();

        //    if (searchResult.Any())
        //    {
        //        return View("GetList", searchResult);
        //    }
            
        //    return View("GetList");
        //}


        [HttpGet]


        public async Task<IActionResult> Edit(Guid id)
        {

            var lst = getapi.GetApi("Image");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Image obj)
        {
            try
            {
              await getapi.UpdateObj(obj, "Image");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
          await  getapi.DeleteObj(id, "Image");
            return RedirectToAction("GetList");

        }
    }
}
