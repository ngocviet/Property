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
    public class ImagesController : Controller
    {
        private readonly ProtalContext _context;
        private readonly IWebHostEnvironment v;

        public ImagesController(ProtalContext context, IWebHostEnvironment v)
        {
            _context = context;
            this.v = v;
        }

        // GET: Admin/Images
        public async Task<IActionResult> Index()
        {
            var protalContext = _context.Images.Where(v=>v.Status == false).Include(i => i.Realty);
            return View(await protalContext.ToListAsync());
        }

        // GET: Admin/Images/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Images == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .Include(i => i.Realty)
                .FirstOrDefaultAsync(m => m.ImageId == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // GET: Admin/Images/Create
        public IActionResult Create()
        {
            ViewData["RealtyId"] = new SelectList(_context.Realties.Where(v=>v.Status==false), "RealtyId", "Title");
            return View();
        }

        // POST: Admin/Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImageId,RealtyId,ImageUrl,Status")] Image image, IFormFile img)
        {
            if (ModelState.IsValid)
            {
                if (img != null && img.Length > 0)
                {
                    string uploadsFolder = Path.Combine(v.WebRootPath, "images");
                    string uniqueFileName = /*Guid.NewGuid().ToString() + "_" + */img.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(fileStream);
                    }
                    image.ImageUrl = uniqueFileName;
                }
                _context.Add(image);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RealtyId"] = new SelectList(_context.Realties.Where(v => v.Status == false), "RealtyId", "Title", image.RealtyId);
            return View(image);
        }

        // GET: Admin/Images/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Images == null)
            {
                return NotFound();
            }

            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            ViewData["RealtyId"] = new SelectList(_context.Realties.Where(v => v.Status == false), "RealtyId", "Title", image.RealtyId);
            return View(image);
        }

        // POST: Admin/Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ImageId,RealtyId,ImageUrl,Status")] Image image)
        {
            image.Status = false;
            if (id != image.ImageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(image);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(image.ImageId))
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
            ViewData["RealtyId"] = new SelectList(_context.Realties.Where(v => v.Status == false), "RealtyId", "RealtyId", image.RealtyId);
            return View(image);
        }

        // GET: Admin/Images/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Images == null)
            {
                return Problem("Entity set 'ProtalContext.Images'  is null.");
            }
            var image = await _context.Images.FindAsync(id);
            if (image != null)
            {
                image.Status = true;
                _context.Update(image);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ImageExists(int id)
        {
          return (_context.Images?.Any(e => e.ImageId == id)).GetValueOrDefault();
        }
    }
}
