using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProdsandCats.Models;

namespace ProdsandCats.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext db;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }
    [HttpGet("")]
    public IActionResult Index()
    {
        List<Product> allProducts = db.Products.ToList();
        return View("Index", allProducts);
    }

    [HttpPost("products/create")]
    public IActionResult CreateProduct(Product newProduct)
    {
        if (!ModelState.IsValid)
        {
            return View("_ProductForm");
        }

        db.Products.Add(newProduct);

        db.SaveChanges();

        return RedirectToAction("Index");
    }
    [HttpGet("products/{id}")]
    public IActionResult ViewProduct(int id)
    {
        Product? product = db.Products.Include(prod => prod.Associations).ThenInclude(a => a.Category).FirstOrDefault(product => product.ProductId == id);
        ViewBag.Cat = db.Categories.Include(c => c.Associations).Where(c => c.Associations.All(Assoc => Assoc.ProductId != id));

        if (product == null)
        {
            return RedirectToAction("Index");
        }
        return View("ProdDetails", product);
    }

    [HttpPost("products/{ProductId}/category")]
    public IActionResult Categorize(int ProductId, int CategoryId)
    {
        Association newCat = new Association()
        {
            ProductId = ProductId,
            CategoryId = CategoryId
        };
        db.Associations.Add(newCat);
        db.SaveChanges();
        return RedirectToAction("Index");
    }


    // Categories
    [HttpGet("categories")]
    public IActionResult Categories()
    {
        List<Category> allCategories = db.Categories.ToList();
        return View("Categories", allCategories);
    }


    [HttpPost("categories")]
    public IActionResult CreateCategory(Category newCategory)
    {
        if (!ModelState.IsValid)
        {
            return View("_CategoryForm");
        }

        db.Categories.Add(newCategory);

        db.SaveChanges();

        return RedirectToAction("categories");
    }

    [HttpGet("categories/{id}")]
    public IActionResult ViewCategory(int id)
    {
        Category? category = db.Categories.Include(cat => cat.Associations).ThenInclude(a => a.Product).FirstOrDefault(category => category.CategoryId == id);
        ViewBag.Prod = db.Products.Include(c => c.Associations).Where(c => c.Associations.All(Assoc => Assoc.CategoryId != id));

        if (category == null)
        {
            return RedirectToAction("Index");
        }
        return View("CatDetails", category);
    }

    [HttpPost("category/{CategoryId}/product")]
    public IActionResult AddProduct(int CategoryId, int ProductId)
    {
    Association newProd = new Association()
    {
        CategoryId = CategoryId,
        ProductId = ProductId
    };
    db.Associations.Add(newProd);
    db.SaveChanges();
    return RedirectToAction("Categories");
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
}