using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductTypesController : Controller
    {
        private ApplicationDbContext _db;


        public ProductTypesController(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {

            return View(_db.ProductTypes.ToList());
        }


        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                _db.ProductTypes.Add(productTypes);
                await _db.SaveChangesAsync();
                TempData["success"] = "Product type added successfully.";
                return RedirectToAction("Index");
            }
            return View(productTypes);
        }

        public IActionResult Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var ProductType = _db.ProductTypes.Find(id);

            if (ProductType == null) {
            
            return NotFound();
            
            }

            return View(ProductType);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                _db.Update(productTypes);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(productTypes);
        }

        public IActionResult Details (int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ProductType = _db.ProductTypes.Find(id);

            if (ProductType == null)
            {

                return NotFound();

            }

            return View(ProductType);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Details (ProductTypes productTypes)
        {
           
                
                return RedirectToAction("Index");
            
            
        }

        public IActionResult Delete (int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ProductType = _db.ProductTypes.Find(id);

            if (ProductType == null)
            {

                return NotFound();

            }

            return View(ProductType);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete (int? id , ProductTypes productTypes)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _db.ProductTypes.FindAsync(id.Value);

            if (productType == null)
            {
                return NotFound();
            }

            _db.ProductTypes.Remove(productType);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}