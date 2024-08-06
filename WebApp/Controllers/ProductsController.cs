using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.ViewModels;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using WebApp.Extensions;

namespace WebApp.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductsController : Controller
    {
        private readonly WebAppContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(WebAppContext context, IWebHostEnvironment webHostEnvironment, ILogger<ProductsController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        // GET: Products 
        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> objList =  _context.Product;
            foreach (var obj in objList)
            {
                obj.Category = await _context.Category.FirstOrDefaultAsync(u => u.Id == obj.CategoryId);
                obj.ListingsType = await _context.ListingsType.FirstOrDefaultAsync(u => u.Id == obj.ListingsTypeId);
            }
                var webAppContext = _context.Product.Include(p => p.Category);
            
            return View(objList);
        }

        

        // GET: Products/Upsert/5
        public async Task<IActionResult> Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _context.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }).ToList(),
                ListingsTypeSelectList = _context.ListingsType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }).ToList(),
                MarketSelectList = Enum.GetValues(typeof(Product.MarketType))
                    .Cast<Product.MarketType>()
                    .Select(e => new SelectListItem
                    {
                        Text = e.GetDisplayName(), 
                        Value = e.ToString()
                    }).ToList(),
                BuildingTypeSelectList = Enum.GetValues(typeof(Product.BuildingType))
                    .Cast<Product.BuildingType>()
                    .Select(e => new SelectListItem
                    {
                        Text = e.GetDisplayName(), 
                        Value = e.ToString()
                    }).ToList()
            };

            if (id == null)
            {
                // Create
                return View(productVM);
            }
            else
            {
                // Edit
                productVM.Product = await _context.Product.FindAsync(id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }


                _logger.LogInformation("Editing Product: {@Product}", productVM.Product);
                return View(productVM);
            }
        }



        // POST: Products/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _webHostEnvironment.ContentRootPath;
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, "wwwroot\\images\\product");
                    var extension = Path.GetExtension(files[0].FileName);


                    if(!System.IO.Directory.Exists(uploads))
                    {
                        System.IO.Directory.CreateDirectory(uploads);
                    }


                    if (productVM.Product.Image != null)
                    {
                        var imagePath = Path.Combine(webRootPath, "wwwroot\\images\\product");
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.Image = fileName + extension;
                }
                else
                {
                    if (productVM.Product.Id != 0)
                    {
                        Product objFromDb = await _context.Product.AsNoTracking().FirstOrDefaultAsync(u => u.Id == productVM.Product.Id);
                        if (objFromDb != null)
                        {
                            productVM.Product.Image = objFromDb.Image;
                        }
                    }
                }

                if (productVM.Product.Id == 0)
                {
                    _context.Product.Add(productVM.Product);
                }
                else
                {
                    _context.Product.Update(productVM.Product);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                }
            }

            productVM.CategorySelectList = _context.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }).ToList();
            productVM.ListingsTypeSelectList = _context.ListingsType.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }).ToList();
            productVM.MarketSelectList = Enum.GetValues(typeof(Product.MarketType))
                    .Cast<Product.MarketType>()
                    .Select(e => new SelectListItem
                    {
                        Text = e.GetDisplayName(),
                        Value = e.ToString()
                    }).ToList();
            productVM.BuildingTypeSelectList = Enum.GetValues(typeof(Product.BuildingType))
                    .Cast<Product.BuildingType>()
                    .Select(e => new SelectListItem
                    {
                        Text = e.GetDisplayName(),
                        Value = e.ToString()
                    }).ToList();

            return View(productVM);
        }




        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.ListingsType)
                .FirstOrDefaultAsync(m => m.Id == id);

            var productVM = new ProductVM
            {
                Product = product,
                MarketSelectList = Enum.GetValues(typeof(Product.MarketType))
                .Cast<Product.MarketType>()
                .Select(e => new SelectListItem
                {
                    Text = e.ToString(),
                    Value = e.ToString(),
                    Selected = e == product.Market // Ustawienie zaznaczenia
                }).ToList(),
                BuildingTypeSelectList = Enum.GetValues(typeof(Product.BuildingType))
                .Cast<Product.BuildingType>()
                .Select(e => new SelectListItem
                {
                    Text = e.ToString(),
                    Value = e.ToString(),
                    Selected = e == product.Building // Ustawienie zaznaczenia
                }).ToList()
            };
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Ścieżka do katalogu, gdzie przechowywane są obrazy
            string webRootPath = _webHostEnvironment.ContentRootPath;
            var imagePath = Path.Combine(webRootPath, "wwwroot\\images\\product", product.Image);

            if (!string.IsNullOrEmpty(product.Image) && System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}