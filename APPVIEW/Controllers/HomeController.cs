
﻿using APPVIEW.Models;
using Microsoft.AspNetCore.Authorization;


using APPVIEW.Services;

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using APPDATA.Models;
using System.Xml.Linq;

namespace APPVIEW.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
       
        private readonly ILogger<HomeController> _logger;
        private Getapi<ProductDetail> getapi;
        private Getapi<Category> getapiCategory;
        private Getapi<Color> getapiColor;
        private Getapi<Image> getapiImg;
        private Getapi<Size> getapiSize;
        private Getapi<Supplier> getapiSupplier;
        private Getapi<Product> getapiProduct;
        private Getapi<Material> getapiMaterial;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            getapi = new Getapi<ProductDetail>();
            getapiCategory = new Getapi<Category>();
            getapiColor = new Getapi<Color>();
            getapiImg = new Getapi<Image>();
            getapiSize = new Getapi<Size>();
            getapiSupplier = new Getapi<Supplier>();
            getapiProduct = new Getapi<Product>();
            getapiMaterial = new Getapi<Material>();
        }

        public IActionResult Index()
        {

            return View();
        } 
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Shop()
        {
            return View();
        }
        public async Task<IActionResult> Details(Guid id)
        {
            ViewBag.PD= getapi.GetApi("ProductDetails");
            var pro = getapi.GetApi("ProductDetails").FirstOrDefault(c => c.Id == id);
            ViewBag.lspd= getapi.GetApi("ProductDetails").Where(c=>c.Id_Product==pro.Id_Product);
            ViewBag.Img = getapiImg.GetApi("Image");
            ViewBag.Size = getapiSize.GetApi("Size");
            ViewBag.Color = getapiColor.GetApi("Color"); 
            var a = getapi.GetApi("ProductDetails").GroupBy(c => c.Id_Color).ToList();
            var b = getapi.GetApi("ProductDetails").GroupBy(c => c.Id_Size).ToList();
            foreach (var group in a)
            {
             
                foreach (var product in b)
                {
                    var lis = group.Intersect(product).ToList();
                }
            }
            var intersect = a.IntersectBy(b, x => x);
            return View(pro);
        }
        public async Task<IActionResult> Checkout()
        {

            return View();
        }
      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}