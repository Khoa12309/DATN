using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace APPVIEW.Controllers
{
    public class ProductDetailController : Controller
    {
        private Getapi<ProductDetail> getapi;
        private Getapi<Category> getapiCategory;
        private Getapi<Color> getapiColor;
        private Getapi<Image> getapiImg;
        private Getapi<Size> getapiSize;
        private Getapi<Supplier> getapiSupplier;
        private Getapi<Product> getapiProduct;
        private Getapi<Material> getapiMaterial;
        
       
        public ProductDetailController()
        {
            getapi = new Getapi<ProductDetail>();
            getapiCategory = new Getapi<Category>();
            getapiColor = new Getapi<Color>();
            getapiImg = new Getapi<Image>();
            getapiSize = new Getapi<Size>();
            getapiSupplier = new Getapi<Supplier>();
            getapiProduct = new Getapi<Product>();
            getapiMaterial = new Getapi<Material>();
          
        }

        public string GetFiltered()
        {
            return "I got the data, here is the feedback to you";
        }
        public async Task<IActionResult> GetList()
        {
            ViewBag.Size =await getapiSize.GetApia("Size");
            ViewBag.Color =await getapiColor.GetApia("Color");
            ViewBag.Category = await getapiCategory.GetApia("Category");
            ViewBag.Supplier = await getapiSupplier.GetApia("Supplier");
            ViewBag.Image = await getapiImg.GetApia("Image");
            ViewBag.Material = await getapiMaterial.GetApia("Material");
            var obj = await getapi.GetApia("ProductDetails");
            return View(obj);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Category = getapiCategory.GetApi("Category");
            ViewBag.Color = getapiColor.GetApi("Color");
            ViewBag.Product = getapiProduct.GetApi("Product");
            ViewBag.Size = getapiSize.GetApi("Size");
            ViewBag.Supplier = getapiSupplier.GetApi("Supplier");
            ViewBag.Image = getapiImg.GetApi("Image");
            ViewBag.Material = getapiMaterial.GetApi("Material");
           
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create( ProductDetail obj, [Bind] List<IFormFile> imageFile )
        {
            try
            {
                
             
                //// truyền nhiều dữ liệu 
                //if (myList!= null)
                //{
                //    List<string> Lcolor = JsonConvert.DeserializeObject<List<string>>(myList);
                //}
                obj.Id = Guid.NewGuid();
                obj.Name = getapiProduct.GetApi("Product").FirstOrDefault(c => c.Id == obj.Id_Product).Name;
                obj.Create_date=DateTime.Now;
                obj.Update_date=DateTime.Now;
                obj.Create_by= DateTime.Now;
                obj.Update_by= DateTime.Now;
                await getapi.CreateObj(obj, "ProductDetails");
                foreach (var item in imageFile)
                {
                    addimg(item, obj.Id);

                }
                return RedirectToAction("GetList");

            }
            catch
            {
                return View();
            }
        }
       public async void addimg(IFormFile formFile,Guid id)
       {
            var anh = getapiImg.GetApi("Image").FirstOrDefault(c => c.IdProductdetail == id);
            var img = new Image()
            {
                Id = Guid.NewGuid(),
                IdProductdetail = id,
                Status = 1,
                Create_date = DateTime.Now,
                Update_date = DateTime.Now,
              
            };
          
            if (formFile != null && formFile.Length > 0) // Không null và không trống
            {
                //Trỏ tới thư mục wwwroot để lát nữa thực hiện việc Copy sang
                var path = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot", "images", formFile.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    // Thực hiện copy ảnh vừa chọn sang thư mục mới (wwwroot)
                    formFile.CopyTo(stream);
                }
                // Gán lại giá trị cho Description của đối tượng bằng tên file ảnh đã được sao chép
                img.Name = formFile.FileName;
            }
          
            if (anh!=null)
            {
                anh.Name = img.Name;
              await  getapiImg.UpdateObj(anh, "Image");
            }
            else
            {               
               await  getapiImg.CreateObj(img, "Image");
            }
           
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.Category = getapiCategory.GetApi("Category");
            ViewBag.Color = getapiColor.GetApi("Color");
            ViewBag.Product = getapiProduct.GetApi("Product");
            ViewBag.Size = getapiSize.GetApi("Size");
            ViewBag.Supplier = getapiSupplier.GetApi("Supplier");
            ViewBag.Image = getapiImg.GetApi("Image");
            ViewBag.Material = getapiMaterial.GetApi("Material");
            var lst = getapi.GetApi("ProductDetails");
            return View(lst.Find(c => c.Id == id));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(ProductDetail obj, [Bind] IFormFile imageFile)
        {
            try
            {
                obj.Update_date = DateTime.Now;
                obj.Update_by = DateTime.Now;
                obj.Name = getapiProduct.GetApi("Product").FirstOrDefault(c => c.Id == obj.Id_Product).Name;
                
              await getapi.UpdateObj(obj, "ProductDetails");
                addimg(imageFile, obj.Id);
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            var img = getapiImg.GetApi("Image").Where(c=>c.IdProductdetail==id);
            if (img!=null)
            {
                foreach (var item in img)
                {
                    await getapiImg.DeleteObj(item.Id, "Image");
                }   
            

            }
          await  getapi.DeleteObj(id, "ProductDetails");
            return RedirectToAction("GetList");

        }


      
    }
}
