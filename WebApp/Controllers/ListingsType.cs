using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class ListingsType : Controller
    {
        private readonly WebAppContext _context;

        public ListingsType(WebAppContext context)
        {
            _context = context;
        }

        // GET: ApplicationTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ListingsType.ToListAsync());
        }

        // GET: ApplicationTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationType = await _context.ListingsType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationType == null)
            {
                return NotFound();
            }

            return View(applicationType);
        }

        // GET: ApplicationTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApplicationTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] WebApp.Models.ListingsType applicationType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(applicationType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(applicationType);
        }

        // GET: ApplicationTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationType = await _context.ListingsType.FindAsync(id);
            if (applicationType == null)
            {
                return NotFound();
            }
            return View(applicationType);
        }

        // POST: ApplicationTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] WebApp.Models.ListingsType applicationType)
        {
            if (id != applicationType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationTypeExists(applicationType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Categories");
            }
            return View(applicationType);
        }

        // GET: ApplicationTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationType = await _context.ListingsType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationType == null)
            {
                return NotFound();
            }

            return View(applicationType);
        }

        // POST: ListingsType/Delete/5
        [HttpPost, ActionName("DeletePost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var applicationType = await _context.ListingsType.FindAsync(id);
            if (applicationType == null)
            {
                return NotFound();
            }
            _context.ListingsType.Remove(applicationType);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Categories");
        }

        private bool ApplicationTypeExists(int id)
        {
            return _context.ListingsType.Any(e => e.Id == id);
        }
    }
}
