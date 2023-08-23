using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Property.Models;

namespace Property.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryDetailsController : Controller
    {
        private readonly ProtalContext _context;

        public CategoryDetailsController(ProtalContext context)
        {
            _context = context;
        }

        // GET: Admin/CategoryDetails
        public async Task<IActionResult> Index()
        {
            var protalContext = _context.CategoryDetails.Where(v=>v.Status == false).Include(c => c.Category);
            return View(await protalContext.ToListAsync());
        }

        // GET: Admin/CategoryDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CategoryDetails == null)
            {
                return NotFound();
            }

            var categoryDetail = await _context.CategoryDetails
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.CategoryDetailId == id);
            if (categoryDetail == null)
            {
                return NotFound();
            }

            return View(categoryDetail);
        }

        // GET: Admin/CategoryDetails/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Admin/CategoryDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryDetailId,CategoryId,CategoryDetailName,Description,Status")] CategoryDetail categoryDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoryDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryDetail.CategoryId);
            return View(categoryDetail);
        }

        // GET: Admin/CategoryDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CategoryDetails == null)
            {
                return NotFound();
            }

            var categoryDetail = await _context.CategoryDetails.FindAsync(id);
            if (categoryDetail == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryDetail.CategoryId);
            return View(categoryDetail);
        }

        // POST: Admin/CategoryDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryDetailId,CategoryId,CategoryDetailName,Description,Status")] CategoryDetail categoryDetail)
        {
            categoryDetail.Status = false;
            if (id != categoryDetail.CategoryDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoryDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryDetailExists(categoryDetail.CategoryDetailId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryDetail.CategoryId);
            return View(categoryDetail);
        }

        // GET: Admin/CategoryDetails/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.CategoryDetails == null)
            {
                return Problem("Entity set 'ProtalContext.CategoryDetails'  is null.");
            }
            var categoryDetail = await _context.CategoryDetails.FindAsync(id);
            if (categoryDetail != null)
            {
                categoryDetail.Status = true;
                _context.Update(categoryDetail);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryDetailExists(int id)
        {
          return (_context.CategoryDetails?.Any(e => e.CategoryDetailId == id)).GetValueOrDefault();
        }
    }
}
