using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Property.Models;
using Property.ViewsModel;

namespace Property.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ListingsController : Controller
    {
        private readonly ProtalContext _context;

        public ListingsController(ProtalContext context)
        {
            _context = context;
        }

        // GET: Admin/Listings
        public async Task<IActionResult> Index(int? amountDuration, string? msss, string? sucedit)
        {
            var protalContext = _context.Listings.Where(v => v.Status == false && v.Duration == false).Include(l => l.Account).Include(l => l.Realty);
            ViewData["list_duration"] = _context.Listings.Where(v => v.Status == false && v.Duration == true).Include(l => l.Account).Include(l => l.Realty);
            if (amountDuration != null)
            {
                ViewBag.amountDuration = "Có " + amountDuration + " bản ghi hết hạn!";
            }
            if (msss != null)
            {
                ViewBag.amountDuration = "Đã gia hạn!";
            }
            if (sucedit != null)
            {
                ViewBag.sucedit = "thanh cong!";
            }
            return View(await protalContext.ToListAsync());
        }
        public IActionResult Restore(int id)
        {
            Listing ls = _context.Listings.Where(v => v.Status == false && v.Duration == true && v.ListingId == id).FirstOrDefault();
            ls.Duration = false;
            ls.CreatedAt = DateTime.Now;
            ls.UpdatedAt = DateTime.Now;
            // sắp xếp vị trí
            List<Listing> lis = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Include(v => v.Account)
                        .ThenInclude(v => v.Package)
                    .OrderByDescending(v => v.Account.Package.Price)
                    .ThenByDescending(v => v.CreatedAt) // Sắp xếp theo ngày mới nhất trước nếu price bằng nhau
                    .ToList();
            int v = 1;
            foreach (Listing itemm in lis)
            {
                itemm.Location = v;
                v++;
            }
            _context.Update(ls);
            _context.SaveChanges();
            return RedirectToAction("Index", new { msss = "v" });
        }
        public IActionResult Exam()
        {
            List<Listing> listings = _context.Listings.ToList();
            int v = 0;
            foreach (Listing item in listings)
            {
                if (item.CreatedAt.HasValue)
                {
                    TimeSpan duration = DateTime.Now - item.CreatedAt.Value;
                    if (duration.Days > 10 && item.Duration == false)
                    {
                        item.Duration = true;
                        v++;
                    }
                }
            }
            _context.SaveChanges();

            return RedirectToAction("Index", new { amountDuration = v });
        }

        // GET: Admin/Listings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }
            var listing = _context.Listings
                .Where(v => v.Status == false && v.Duration == false && v.ListingId == id)
                .Include(v => v.Account)
                .Include(v => v.Realty)
                    .ThenInclude(v => v.Images.Where(img => img.Status == false))
                .Include(v => v.Realty.CategoryDetail)
                    .ThenInclude(c => c.Category)
                .FirstOrDefault();

            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // GET: Admin/Listings/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts.Where(v => v.Status == false), "AccountId", "Name");
            ViewData["CategoryDetailId"] = new SelectList(_context.CategoryDetails.Where(v => v.Status == false), "CategoryDetailId", "CategoryDetailName");
            ViewData["RealtyId"] = new SelectList(_context.Realties.Where(v => v.Status == false), "RealtyId", "Title");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Property.ViewsModel.CreateListingViewModel model, string? direction)
        {
            if (ModelState.IsValid)
            {
                Account account = _context.Accounts.Where(v => v.AccountId == model.Listing.AccountId && v.Status == false).FirstOrDefault();
                if (account.NumberPost <= 0)
                {
                    ViewBag.er = "Tài khoản hết lượt đăng!";
                    ViewData["CategoryDetailId"] = new SelectList(_context.CategoryDetails.Where(v => v.Status == false), "CategoryDetailId", "CategoryDetailName");
                    ViewData["AccountId"] = new SelectList(_context.Accounts.Where(v => v.Status == false), "AccountId", "Name", model.Listing.AccountId);

                    return View(model);
                }
                List<Listing> lis = _context.Listings.Where(v => v.Status == false).ToList();
                foreach (var item in lis)
                {
                    if (item.Title.Equals(model.Listing.Title))
                    {
                        ViewBag.trungtieudelis = "v!";
                        ViewData["CategoryDetailId"] = new SelectList(_context.CategoryDetails.Where(v => v.Status == false), "CategoryDetailId", "CategoryDetailName");
                        ViewData["AccountId"] = new SelectList(_context.Accounts.Where(v => v.Status == false), "AccountId", "Name", model.Listing.AccountId);

                        return View(model);
                    }
                }
                List<Realty> liss = _context.Realties.Where(v => v.Status == false).ToList();
                foreach (var item in liss)
                {
                    if (item.Title.Equals(model.Realty.Title))
                    {
                        ViewBag.trungtieuderea = "v!";
                        ViewData["CategoryDetailId"] = new SelectList(_context.CategoryDetails.Where(v => v.Status == false), "CategoryDetailId", "CategoryDetailName");
                        ViewData["AccountId"] = new SelectList(_context.Accounts.Where(v => v.Status == false), "AccountId", "Name", model.Listing.AccountId);

                        return View(model);
                    }
                }
                var relty = new Realty
                {
                    Address = model.Realty.Address,
                    Area = model.Realty.Area,
                    Bathrooms = model.Realty.Bathrooms,
                    Bedrooms = model.Realty.Bedrooms,
                    CategoryDetailId = model.Realty.CategoryDetailId,
                    CreatedAt = DateTime.Now,
                    HouseDirection = model.Realty.HouseDirection,
                    Interior = model.Realty.Interior,
                    Price = model.Realty.Price,
                    Status = false,
                    Title = model.Realty.Title
                };
                _context.Add(relty);
                await _context.SaveChangesAsync();
                var listing = new Listing
                {
                    RealtyId = relty.RealtyId,
                    AccountId = model.Listing.AccountId,
                    Title = model.Listing.Title,
                    Description = model.Listing.Description,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = false,
                    ViewAmount = 0,
                    Duration = false,
                    Location = 1
                };
                _context.Add(listing);
                //await _context.SaveChangesAsync();

                account.NumberPost -= 1;
                _context.Update(account);
                //_context.SaveChanges();

                var imgs = model.Images.Split("..").Distinct().Skip(1).ToArray();
                foreach (var item in imgs)
                {
                    var img = new Image
                    {
                        RealtyId = relty.RealtyId,
                        ImageUrl = item
                    };
                    _context.Add(img);
                    // sắp xếp vị trí
                    List<Listing> list = _context.Listings
                            .Where(v => v.Status == false && v.Duration == false)
                            .Include(v => v.Account)
                                .ThenInclude(v => v.Package)
                            .OrderByDescending(v => v.Account.Package.Price)
                            .ThenByDescending(v => v.CreatedAt)
                            .ToList();
                    int v = 1;
                    foreach (Listing itemm in list)
                    {
                        itemm.Location = v;
                        v++;
                    }
                    await _context.SaveChangesAsync();

                }
                if (direction.Equals("user"))
                {
                    return RedirectToAction("chitietbds", "Home", new { area = "", id = listing.ListingId });
                }
                return RedirectToAction("Index");
            }
            else if (direction != null)
            {
                var check = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "iduser");
                var accountt = _context.Accounts.Where(v => v.Status == false && v.AccountId == Int32.Parse(check.Value)).FirstOrDefault();

                return RedirectToAction("Listing", "Home", new { area = "", idaccount = accountt.AccountId });
            }
            ViewBag.thieuthongtin = "r";
            ViewData["CategoryDetailId"] = new SelectList(_context.CategoryDetails.Where(v => v.Status == false), "CategoryDetailId", "CategoryDetailName");
            ViewData["AccountId"] = new SelectList(_context.Accounts.Where(v => v.Status == false), "AccountId", "Name", model.Listing.AccountId);

            return View(model);
        }

        // GET: Admin/Listings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }
            Listing listing = await _context.Listings.FindAsync(id);
            Account account = _context.Accounts.Where(v => v.Status == false && v.AccountId == listing.AccountId).FirstOrDefault();
            Realty realty = await _context.Realties.FindAsync(listing.RealtyId);
            List<Image> imgs = await _context.Images.Where(v => v.RealtyId == realty.RealtyId && v.Status == false).ToListAsync();
            string images = "";
            foreach (Image image in imgs)
            {
                images = ".." + image.ImageUrl;
            }
            CreateListingViewModel createListingViewModel = new CreateListingViewModel
            {
                Realty = realty,
                Listing = listing,
                Images = images
            };

            listing = await _context.Listings.FindAsync(id);
            if (listing == null)
            {
                return NotFound();
            }
            ViewBag.theloai = _context.CategoryDetails.Where(v => v.Status == false && v.CategoryDetailId == realty.CategoryDetailId).FirstOrDefault().CategoryDetailName;
            ViewData["AccountId"] = new SelectList(_context.Accounts.Where(v => v.Status == false), "AccountId", "Name", listing.AccountId);
            ViewData["RealtyId"] = new SelectList(_context.Realties.Where(v => v.Status == false), "RealtyId", "Title", listing.RealtyId);
            return View(createListingViewModel);
        }

        // POST: Admin/Listings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Property.ViewsModel.CreateListingViewModel model)
        {
            model.Listing.Status = false;
            model.Realty.Status = false;
            model.Listing.UpdatedAt = DateTime.Now;

            if (model.Listing.Title != null && model.Listing.Description != null && model.Realty.Title != null && model.Realty.Area != null && model.Realty.Address != null)
            {
                try
                {
                    _context.Update(model.Listing);
                    await _context.SaveChangesAsync();

                    _context.Update(model.Realty);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index), new { sucedit = "v" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return RedirectToAction("Error");
                }
            }
            ViewBag.thieu = "v";
            ViewBag.theloai = _context.CategoryDetails.Where(v => v.Status == false && v.CategoryDetailId == model.Realty.CategoryDetailId).FirstOrDefault().CategoryDetailName;
            ViewData["AccountId"] = new SelectList(_context.Accounts.Where(v => v.Status == false), "AccountId", "Name", model.Listing.AccountId);
            ViewData["RealtyId"] = new SelectList(_context.Realties.Where(v => v.Status == false), "RealtyId", "Title", model.Listing.RealtyId);
            return View(model);
        }

        // GET: Admin/Listings/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Listings == null)
            {
                return Problem("Entity set 'ProtalContext.Listings'  is null.");
            }
            var listing = await _context.Listings.FindAsync(id);
            if (listing != null)
            {
                listing.Status = true;
                _context.Update(listing);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListingExists(int id)
        {
            return (_context.Listings?.Any(e => e.ListingId == id)).GetValueOrDefault();
        }
    }
}
