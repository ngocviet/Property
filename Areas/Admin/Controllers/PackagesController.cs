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
    public class PackagesController : Controller
    {
        private readonly ProtalContext _context;

        public PackagesController(ProtalContext context)
        {
            _context = context;
        }

        // GET: Admin/Packages
        public async Task<IActionResult> Index(String? mes)
        {
            if (mes!=null)
            {
                ViewBag.er = "v";
            }

              return _context.Packages != null ? 
                          View(await _context.Packages.Where(v=>v.Status==false).ToListAsync()) :
                          Problem("Entity set 'ProtalContext.Packages'  is null.");
        }

        // GET: Admin/Packages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Packages == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .FirstOrDefaultAsync(m => m.PackageId == id);
            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }

        // GET: Admin/Packages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Packages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PackageId,PackageName,Price,NumberPost,Status")] Package package)
        {
            if (ModelState.IsValid)
            {
                _context.Add(package);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(package);
        }

        // GET: Admin/Packages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Packages == null)
            {
                return NotFound();
            }

            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }
            return View(package);
        }

        // POST: Admin/Packages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PackageId,PackageName,Price,NumberPost,Status")] Package package)
        {
            package.Status = false;
            if (id != package.PackageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(package);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackageExists(package.PackageId))
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
            return View(package);
        }

        // GET: Admin/Packages/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Packages == null)
            {
                return Problem("Entity set 'ProtalContext.Packages'  is null.");
            }
            List<Account> lis = _context.Accounts.Where(v=>v.Status==false).ToList();
            foreach (var item in lis)
            {
                if (item.PackageId==id)
                {
                    return RedirectToAction("Index", new { mes = "V" });
                }
            }
            var package = await _context.Packages.FindAsync(id);
            if (package != null)
            {
                package.Status = true;
                _context.Update(package);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PackageExists(int id)
        {
          return (_context.Packages?.Any(e => e.PackageId == id)).GetValueOrDefault();
        }
    }
}
