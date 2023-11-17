﻿using APPDATA.Models;
using APPVIEW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APPVIEW.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ProductController : Controller
    {
        private Getapi<Product> getapi;
        public ProductController()
        {
            getapi = new Getapi<Product>();
        }

        public async Task<IActionResult> GetList()
        {
            var obj = getapi.GetApi("Product");
            return View(obj);
        }

      
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SupplierController1/Create
        [HttpPost]
        public async Task<IActionResult> Create(Product obj)
        {
            try
            {
               await getapi.CreateObj(obj, "Product");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]

      
        public async Task<IActionResult> Edit(Guid id)
        {

            var lst = getapi.GetApi("Product");
            return View(lst.Find(c => c.Id == id));
        }

    
        [HttpPost]
        public async Task<IActionResult> Edit(Product obj)
        {
            try
            {
               await getapi.UpdateObj(obj, "Product");
                return RedirectToAction("GetList");
            }
            catch
            {
                return View();
            }
        }

     
        public async Task<IActionResult> Delete(Guid id)
        {
           await getapi.DeleteObj(id, "Product");
            return RedirectToAction("GetList");

        }
    }
}
