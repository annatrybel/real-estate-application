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

namespace WebApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WebAppContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(WebAppContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products 
        public async Task<IActionResult> Index()
        {
            var webAppContext = _context.Product.Include(p => p.Category);
            if (webAppContext == null)
            {
                return NotFound();
            }
            return View(await webAppContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
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

            return View(productVM);
        }




        // POST: Products/Edit        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Image,CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "DisplayOrder", product.CategoryId);
            return View(product);
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}