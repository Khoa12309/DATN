﻿using APPDATA.Models;
using APPVIEW.Services;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using X.PagedList;

namespace APPVIEW.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public INotyfService _notyf;


        public ProductDetailController(INotyfService notyf)
        {
            _notyf = notyf;
            getapi = new Getapi<ProductDetail>();
            getapiCategory = new Getapi<Category>();
            getapiColor = new Getapi<Color>();
            getapiImg = new Getapi<Image>();
            getapiSize = new Getapi<Size>();
            getapiSupplier = new Getapi<Supplier>();
            getapiProduct = new Getapi<Product>();
            getapiMaterial = new Getapi<Material>();

        }
        public int PageSize = 5;
        public async Task<IActionResult> GetList(int? page)
        {
            ViewBag.Size = await getapiSize.GetApia("Size");
            ViewBag.Color = await getapiColor.GetApia("Color");
            ViewBag.Category = await getapiCategory.GetApia("Category");
            ViewBag.Supplier = await getapiSupplier.GetApia("Supplier");
            ViewBag.Image = await getapiImg.GetApia("Image");
            ViewBag.Material = await getapiMaterial.GetApia("Material");
            var obj = await getapi.GetApia("ProductDetails");
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(obj.OrderByDescending(x => x.Create_date).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public async Task<IActionResult> GetList(string tk, int? page)
        {
            ViewBag.Size = await getapiSize.GetApia("Size");
            ViewBag.Color = await getapiColor.GetApia("Color");
            ViewBag.Category = await getapiCategory.GetApia("Category");
            ViewBag.Supplier = await getapiSupplier.GetApia("Supplier");
            ViewBag.Image = await getapiImg.GetApia("Image");
            ViewBag.Material = await getapiMaterial.GetApia("Material");
            var obj = getapi.GetApi("ProductDetails").Where(c => c.Name.ToLower().Contains(tk.ToLower()));

            int pageSize = 8;
            int pageNumber = (page ?? 1);
            if (tk == null)
            {
                obj = await getapi.GetApia("ProductDetails");
            }
            return View(obj.OrderByDescending(x => x.Create_date).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Category = getapiCategory.GetApi("Category").Where(c => c.Status == 1);
            ViewBag.Color = getapiColor.GetApi("Color").Where(c => c.Status == 1);
            ViewBag.Product = getapiProduct.GetApi("Product").Where(c => c.Status == 1);
            ViewBag.Size = getapiSize.GetApi("Size").Where(c => c.Status == 1);
            ViewBag.Supplier = getapiSupplier.GetApi("Supplier");
            ViewBag.Material = getapiMaterial.GetApi("Material").Where(c => c.Status == 1);
            ViewBag.Image = getapiImg.GetApi("Image");

            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(ProductDetail obj, [Bind] List<IFormFile> imageFile)
        {
            try
            {


                //// truyền nhiều dữ liệu 
                //if (myList!= null)
                //{
                //    List<string> Lcolor = JsonConvert.DeserializeObject<List<string>>(myList);
                //}

                var produ = getapiProduct.GetApi("Product").FirstOrDefault(c => c.Id == obj.Id_Product);
                obj.Id = Guid.NewGuid();
                obj.Name = produ.Name;
                obj.Create_date = DateTime.Now;
                obj.Update_date = DateTime.Now;
                obj.Create_by = DateTime.Now;
                obj.Update_by = DateTime.Now;
                var PD = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id_Product == produ.Id && c.Id_Color == obj.Id_Color && c.Id_Size == obj.Id_Size);
                if (PD != null)
                {
                    PD.Quantity += obj.Quantity;
                    await getapi.UpdateObj(PD, "ProductDetails");
                    foreach (var item in imageFile)
                    {
                        addimg(item, PD.Id);

                    }
                    _notyf.Success("Thêm thành công!");
                    return RedirectToAction("GetList");
                }

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
        public async void addimg(IFormFile formFile, Guid id)
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

            if (anh != null)
            {
                anh.Name = img.Name;
                await getapiImg.UpdateObj(anh, "Image");
            }
            else
            {
                await getapiImg.CreateObj(img, "Image");
            }

        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewBag.Category = getapiCategory.GetApi("Category").Where(c => c.Status == 1);
            ViewBag.Color = getapiColor.GetApi("Color").Where(c => c.Status == 1);
            ViewBag.Product = getapiProduct.GetApi("Product").Where(c => c.Status == 1);
            ViewBag.Size = getapiSize.GetApi("Size").Where(c => c.Status == 1);
            ViewBag.Supplier = getapiSupplier.GetApi("Supplier");
            ViewBag.Material = getapiMaterial.GetApi("Material").Where(c => c.Status == 1);
            ViewBag.Image = getapiImg.GetApi("Image");
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

                var item = getapi.UpdateObj(obj, "ProductDetails").Result;
                if (item != null)
                {
                    _notyf.Success("Edit thành công!");
                    addimg(imageFile, obj.Id);
                    return RedirectToAction("GetList");
                }
                else
                {
                    _notyf.Warning("Không được để trống!");
                    return View();
                }

            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            var img = getapiImg.GetApi("Image").Where(c => c.IdProductdetail == id);
            if (img != null)
            {
                foreach (var item in img)
                {
                    await getapiImg.DeleteObj(item.Id, "Image");
                }


            }
            await getapi.DeleteObj(id, "ProductDetails");
            _notyf.Success("Đã xóa!");
            return RedirectToAction("GetList");

        }
        public async Task<IActionResult> search(string tk)
        {

            return RedirectToAction("getlist");
        }



    }
}
