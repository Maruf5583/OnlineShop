using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "super user")]
    public class SpecialTagController : Controller
    {
        private ApplicationDbContext _db;


        public SpecialTagController(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {

            return View(_db.SpecialTags.ToList());
        }


        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTag specialTag)
        {
            if (ModelState.IsValid)
            {
                _db.SpecialTags.Add(specialTag);
                await _db.SaveChangesAsync();
                TempData["success"] = "Special tag added successfully.";
                return RedirectToAction("Index");
            }
            return View(specialTag);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ProductType = _db.SpecialTags.Find(id);

            if (ProductType == null)
            {

                return NotFound();

            }

            return View(ProductType);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SpecialTag specialTag)
        {
            if (ModelState.IsValid)
            {
                _db.Update(specialTag);
                await _db.SaveChangesAsync();
                TempData["success"] = "Special tag updated successfully.";
                return RedirectToAction("Index");
            }
            return View(specialTag);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ProductType = _db.SpecialTags.Find(id);

            if (ProductType == null)
            {

                return NotFound();

            }

            return View(ProductType);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(SpecialTag specialTag)
        {


            return RedirectToAction("Index");


        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ProductType = _db.SpecialTags.Find(id);

            if (ProductType == null)
            {

                return NotFound();

            }

            return View(ProductType);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, SpecialTag specialTag)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _db.SpecialTags.FindAsync(id.Value);

            if (productType == null)
            {
                return NotFound();
            }

            _db.SpecialTags.Remove(productType);
            await _db.SaveChangesAsync();
            TempData["success"] = "Special tag deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
