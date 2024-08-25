using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.ViewModels;
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
                obj.Category = await _context.Category.AsNoTracking().FirstOrDefaultAsync(u => u.Id == obj.CategoryId);
                obj.ListingsType = await _context.ListingsType.AsNoTracking().FirstOrDefaultAsync(u => u.Id == obj.ListingsTypeId);
                obj.Images = await _context.ProductImage.AsNoTracking().Where(u => u.ProductId == obj.Id).ToListAsync();
            }
                var webAppContext = _context.Product.AsNoTracking().Include(p => p.Category);
            
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
                    }).ToList(),
                StatusSelectList = Enum.GetValues(typeof(Product.ItemStatus))
                    .Cast<Product.ItemStatus>()
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

                return View(productVM);
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    if (productVM.Product.Images.Any())
                    {
                        foreach (var image in productVM.Product.Images.ToList())
                        {
                            var oldImagePath = Path.Combine(webRootPath, "images", "products", image.ImageUrl);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        productVM.Product.Images.Clear();
                    }

                    foreach (var file in files)
                    {
                        // Generuje unikalną nazwę dla pliku
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(webRootPath, "images", "products");
                        var extension = Path.GetExtension(file.FileName);

                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                        }

                        using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        var productImage = new ProductImage
                        {
                            ImageUrl = fileName + extension,
                            Product = productVM.Product
                        };

                        productVM.Product.Images.Add(productImage);
                    }
                }
                else
                {
                    if (productVM.Product.Id != 0)
                    {
                        Product objFromDb = await _context.Product
                            .Include(p => p.Images) 
                            .FirstOrDefaultAsync(u => u.Id == productVM.Product.Id);

                        if (objFromDb != null)
                        {
                            productVM.Product.Images = objFromDb.Images;
                        }
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

                productVM.StatusSelectList = Enum.GetValues(typeof(Product.ItemStatus))
                    .Cast<Product.ItemStatus>()
                    .Select(e => new SelectListItem
                    {
                        Text = e.GetDisplayName(),
                        Value = e.ToString()
                    }).ToList();

                
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
                .Include(p => p.Images)
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
                    Selected = e == product.Market 
                }).ToList(),
                BuildingTypeSelectList = Enum.GetValues(typeof(Product.BuildingType))
                .Cast<Product.BuildingType>()
                .Select(e => new SelectListItem
                {
                    Text = e.ToString(),
                    Value = e.ToString(),
                    Selected = e == product.Building 
                }).ToList(),
                StatusSelectList = Enum.GetValues(typeof(Product.ItemStatus))
                .Cast<Product.ItemStatus>()
                .Select(e => new SelectListItem
                {
                    Text = e.ToString(),
                    Value = e.ToString(),
                    Selected = e == product.Status 
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
            var product = await _context.Product
                .Include(p => p.Images) 
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            var imagesFolderPath = Path.Combine(webRootPath, "images", "products");

            foreach (var image in product.Images.ToList())
            {
                var imagePath = Path.Combine(imagesFolderPath, image.ImageUrl);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath); 
                }

                _context.ProductImage.Remove(image); 
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