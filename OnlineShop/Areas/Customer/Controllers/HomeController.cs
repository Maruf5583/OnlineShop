using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using System.Diagnostics;
using OnlineShop.Utility;



namespace OnlineShop.Areas.Customer.Controllers
{

    [Area("Customer")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTag).ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTag).FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }



            return View(product);
        }

        [HttpPost]
        [ActionName("Details")]
        public IActionResult ProductDetails(int? id)
        {
            List<Products> products = new List<Products>();
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTag).FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            products = HttpContext.Session.Get<List<Products>>("products");
            if (products == null)
            {


                products = new List<Products>();
            }

            products.Add(product);
            HttpContext.Session.Set("products", products);
            return View(product);
        }

        [HttpPost]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var product = _db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTag).FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var products = HttpContext.Session.Get<List<Products>>("products");
            if (products == null)
            {
                products = new List<Products>();
            }

            for (int i = 0; i < Math.Max(1, quantity); i++)
            {
                products.Add(product);
            }

            HttpContext.Session.Set("products", products);

            // Redirect back to the product details page
            return RedirectToAction("Details", new { id });
        }


        [ActionName("Remove")]
        public IActionResult RemoveToCart(int? id)
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products != null)
            {
                var product = products.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]

        public IActionResult Remove(int? id)
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products != null)
            {
                var product = products.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);
                }
            }
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Cart()
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");

            if (products == null)
            {
                products = new List<Products>();
            }
            return View(products);

        }
    }
}
