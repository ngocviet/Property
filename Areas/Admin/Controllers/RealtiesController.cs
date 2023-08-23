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
    public class RealtiesController : Controller
    {
        private readonly ProtalContext _context;

        public RealtiesController(ProtalContext context)
        {
            _context = context;
        }

        // GET: Admin/Realties
        public async Task<IActionResult> Index(string? mss)
        {
            var protalContext = _context.Realties.Where(v => v.Status == false).Include(r => r.CategoryDetail);
            if (mss != null)
            {
                ViewBag.er = "v";
            }
            return View(await protalContext.ToListAsync());
        }

        // GET: Admin/Realties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Realties == null)
            {
                return NotFound();
            }

            var realty = await _context.Realties
                .Include(r => r.CategoryDetail)
                    .ThenInclude(r=>r.Category)
                .Include(r => r.Images.Where(v=>v.Status==false))
                .Include(r => r.Listings.Where(v => v.Status == false))
                .FirstOrDefaultAsync(m => m.RealtyId == id);
            if (realty == null)
            {
                return NotFound();
            }

            return View(realty);
        }

        // GET: Admin/Realties/Create
        public IActionResult Create()
        {
            ViewData["CategoryDetailId"] = new SelectList(_context.CategoryDetails.Where(v => v.Status == false), "CategoryDetailId", "CategoryDetailName");
            return View();
        }

        // POST: Admin/Realties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RealtyId,Title,Description,AddressId,Price,Bedrooms,Bathrooms,HouseDirection,Interior,Area,Status,CreatedAt,CategoryDetailId")] Realty realty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(realty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryDetailId"] = new SelectList(_context.CategoryDetails.Where(v => v.Status == false), "CategoryDetailId", "CategoryDetailName", realty.CategoryDetailId);
            return View(realty);
        }

        // GET: Admin/Realties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Realties == null)
            {
                return NotFound();
            }

            var realty = await _context.Realties.FindAsync(id);
            if (realty == null)
            {
                return NotFound();
            }
            ViewData["CategoryDetailId"] = new SelectList(_context.CategoryDetails.Where(v => v.Status == false), "CategoryDetailId", "CategoryDetailName", realty.CategoryDetailId);
            return View(realty);
        }

        // POST: Admin/Realties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RealtyId,Title,Description,Address,Price,Bedrooms,Bathrooms,HouseDirection,Interior,Area,Status,CreatedAt,CategoryDetailId")] Realty realty)
        {
            realty.Status = false;
            if (id != realty.RealtyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(realty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RealtyExists(realty.RealtyId))
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
            ViewData["CategoryDetailId"] = new SelectList(_context.CategoryDetails.Where(v => v.Status == false), "CategoryDetailId", "CategoryDetailName", realty.CategoryDetailId);
            return View(realty);
        }

        // GET: Admin/Realties/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Realties == null)
            {
                return Problem("Entity set 'ProtalContext.Realties'  is null.");
            }
            List<Listing> lis = _context.Listings.Where(v => v.Status == false).ToList();
            foreach (var item in lis)
            {
                if (item.RealtyId == id)
                {
                    ViewBag.er = "v";
                    return RedirectToAction("Index", new { mss = "v" });
                }
            }
            var realty = await _context.Realties.FindAsync(id);
            if (realty != null)
            {
                realty.Status = true;
                _context.Update(realty);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RealtyExists(int id)
        {
            return (_context.Realties?.Any(e => e.RealtyId == id)).GetValueOrDefault();
        }
    }
}
