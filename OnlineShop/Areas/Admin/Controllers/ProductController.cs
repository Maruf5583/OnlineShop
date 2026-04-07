using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Data;
using OnlineShop.Models;
using Microsoft.AspNetCore.Authorization;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "super user")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _db.Products
                .Include(p => p.ProductTypes)
                .Include(p => p.SpecialTag)
                .ToListAsync();

            return View(products);
        }

        [HttpPost]
        public IActionResult Index (decimal? LowAmount , decimal? LargeAmount)
        {

            var products = _db.Products
                .Include(p => p.ProductTypes)
                .Include(p => p.SpecialTag)
                .Where(p => p.Price >= LowAmount && p.Price <= LargeAmount)
                .ToList();

            if (LargeAmount == null || LowAmount == null) { 
            
            products = _db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTag).ToList();


            }
            return View(products);
        }

        public IActionResult Create()
        {
            PopulateSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Products product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var searchProduct = _db.Products.FirstOrDefault(c => c.Name == product.Name);

                if (searchProduct != null) { 
                
                ViewBag.message = "This product already exists.";
                    
                    PopulateSelectLists();

                    return View(product);

                }

                 
                // handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploads = Path.Combine(_hostEnvironment.WebRootPath, "images", "products");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploads, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    product.Image = fileName;
                }

                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                TempData["success"] = "Product added successfully.";
                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists();
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _db.Products.FindAsync(id.Value);
            if (product == null) return NotFound();

            PopulateSelectLists();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Products product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var entity = await _db.Products.FindAsync(product.Id);
                if (entity == null) return NotFound();

             
                var searchProduct = _db.Products.FirstOrDefault(c =>
                    c.Name == product.Name && c.Id != product.Id);

                if (searchProduct != null)
                {
                    ViewBag.message = "This product already exists.";
                    PopulateSelectLists();
                    return View(product);
                }

               
                if (imageFile != null && imageFile.Length > 0)
                {
                    // delete old
                    if (!string.IsNullOrEmpty(entity.Image))
                    {
                        var oldPath = Path.Combine(_hostEnvironment.WebRootPath, "images", "products", entity.Image);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    var uploads = Path.Combine(_hostEnvironment.WebRootPath, "images", "products");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploads, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    entity.Image = fileName;
                }

                // update other fields
                entity.Name = product.Name;
                entity.Price = product.Price;
                entity.ProductColor = product.ProductColor;
                entity.IsAvaliable = product.IsAvaliable;
                entity.ProductTypeId = product.ProductTypeId;
                entity.SpecialTagId = product.SpecialTagId;

                _db.Update(entity);
                await _db.SaveChangesAsync();
                TempData["success"] = "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists();
            return View(product);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _db.Products
                .Include(p => p.ProductTypes)
                .Include(p => p.SpecialTag)
                .FirstOrDefaultAsync(p => p.Id == id.Value);

            if (product == null) return NotFound();

            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _db.Products
                .Include(p => p.ProductTypes)
                .Include(p => p.SpecialTag)
                .FirstOrDefaultAsync(p => p.Id == id.Value);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Products product)
        {
            var entity = await _db.Products.FindAsync(id);
            if (entity == null) return NotFound();

            // delete image file if exists
            if (!string.IsNullOrEmpty(entity.Image))
            {
                var path = Path.Combine(_hostEnvironment.WebRootPath, "images", "products", entity.Image);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }

            _db.Products.Remove(entity);
            await _db.SaveChangesAsync();
            TempData["success"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateSelectLists()
        {
            ViewData["ProductTypes"] = new SelectList(_db.ProductTypes.ToList(), "Id", "ProductType");
            ViewData["SpecialTags"] = new SelectList(_db.SpecialTags.ToList(), "Id", "TagName");
        }
    }
}
