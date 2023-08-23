using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Property.Models;
using Property.Unitity;
using Property.ViewsModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Property.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProtalContext _context;
        public HomeController(ILogger<HomeController> logger, ProtalContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            List<Listing> listings = _context.Listings
                 .Where(v => v.Status == false && v.Duration == false)
                 .Include(v => v.Account)
                 .Include(v => v.Realty)
                     .ThenInclude(v => v.Images.Where(img => img.Status == false))
                 .Include(v => v.Realty.CategoryDetail)
                     .ThenInclude(c => c.Category)
                 .OrderBy(v => v.Location)
                 .Take(9)
                 .ToList();

            ViewData["Var1"] = listings.Take(3).ToList();
            ViewData["Var2"] = listings.Skip(3).Take(3).ToList();
            ViewData["Var3"] = listings.Skip(6).Take(3).ToList();
            ViewData["list_category_realties"] = _context.CategoryDetails.Where(v => v.Status == false).ToList();
            return View(listings);
        }
        public IActionResult chitietbds(int id)
        {
            var listing = _context.Listings
                .Where(v => v.Status == false && v.Duration == false && v.ListingId == id)
                .Include(v => v.Account)
                    .ThenInclude(v => v.Package)
                .Include(v => v.Realty)
                    .ThenInclude(v => v.Images.Where(img => img.Status == false))
                .Include(v => v.Realty.CategoryDetail)
                    .ThenInclude(c => c.Category)
                .OrderBy(v => v.Location)
                .FirstOrDefault();
            listing.ViewAmount += 1;
            _context.Update(listing);
            _context.SaveChanges();

            var listingOfAccount = _context.Listings
                .Where(v => v.Status == false && v.Duration == false && v.AccountId == listing.AccountId).ToList();
            ViewBag.mountListingofaccount = listingOfAccount.Count();
            listingOfAccount = _context.Listings
                .Where(v => v.Status == false && v.Duration == false && v.AccountId == listing.AccountId && v.ListingId != id)
                .Include(v => v.Account)
                    .ThenInclude(v => v.Package)
                .Include(v => v.Realty)
                    .ThenInclude(v => v.Images.Where(img => img.Status == false))
                .Include(v => v.Realty.CategoryDetail)
                    .ThenInclude(c => c.Category)
                .OrderBy(v => v.Location)
                .ToList();
            ViewBag.accountId = listing.AccountId;
            ViewData["list_category_realties"] = _context.CategoryDetails.Where(v => v.Status == false).ToList();
            ViewData["listingOfAccount"] = listingOfAccount.Take(3);
            return View(listing);
        }
        [Route("showall.html", Name = "showall")]
        public IActionResult ShowAll(int page)
        {
            int pageSize = 6;
            if (page == null || page < 1)
            {
                page = 1;
            }
            var listings = _context.Listings
                .Where(v => v.Status == false && v.Duration == false)
                .ToList();
            int totalPageCount = (int)Math.Ceiling((double)listings.Count / pageSize);
            ViewBag.count = totalPageCount;
            if (page > totalPageCount)
            {
                page = totalPageCount;
            }
            listings = _context.Listings
                .Where(v => v.Status == false && v.Duration == false)
                .Include(v => v.Account)
                .Include(v => v.Realty)
                    .ThenInclude(v => v.Images.Where(img => img.Status == false))
                .Include(v => v.Realty.CategoryDetail)
                    .ThenInclude(c => c.Category)
                .OrderBy(v => v.Location)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            ViewData["list_category_realties"] = _context.CategoryDetails.Where(v => v.Status == false).ToList();

            return View(listings);
        }
        public IActionResult TypeListing(int idCategoryDetail, string? v)
        {
            List<Listing> listings = new List<Listing>();
            if (v != null)
            {
                if (v.Equals("newlisting"))
                {
                    listings = _context.Listings
                .Where(v => v.Status == false && v.Duration == false)
                .Where(v => v.Status == false)
                .Include(v => v.Account)
                .Include(v => v.Realty)
                    .ThenInclude(v => v.Images.Where(img => img.Status == false))
                .Include(v => v.Realty.CategoryDetail)
                    .ThenInclude(c => c.Category)
                .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail)
                .OrderByDescending(v => v.CreatedAt)
                .ToList();
                }
                else if (v.Equals("viewamount"))
                {
                    listings = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Where(v => v.Status == false)
                    .Include(v => v.Account)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail)
                    .OrderByDescending(v => v.ViewAmount)
                    .ToList();
                }
                else if (v.Equals("price"))
                {
                    listings = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Where(v => v.Status == false)
                    .Include(v => v.Account)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail)
                    .OrderBy(v => v.Realty.Price)
                    .ToList();
                }
                else if (v.Equals("pricede"))
                {
                    listings = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Where(v => v.Status == false)
                    .Include(v => v.Account)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail)
                    .OrderByDescending(v => v.Realty.Price)
                    .ToList();
                }
                else if (v.Equals("area"))
                {
                    listings = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Where(v => v.Status == false)
                    .Include(v => v.Account)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail)
                    .OrderBy(v => v.Realty.Area)
                    .ToList();
                }
                else if (v.Equals("areade"))
                {
                    listings = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Where(v => v.Status == false)
                    .Include(v => v.Account)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail)
                    .OrderByDescending(v => v.Realty.Area)
                    .ToList();
                }
                else if (v.Equals("supprice"))
                {
                    listings = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Where(v => v.Status == false)
                    .Include(v => v.Account)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail)
                    .OrderBy(v => v.Realty.Price / v.Realty.Area)
                    .ToList();
                }
                else if (v.Equals("suppricede"))
                {
                    listings = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Where(v => v.Status == false)
                    .Include(v => v.Account)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail)
                    .OrderByDescending(v => v.Realty.Price / v.Realty.Area)
                    .ToList();
                }
                else if (v.Equals("priced3"))
                {
                    listings = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Where(v => v.Status == false)
                    .Include(v => v.Account)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail && v.Realty.Price < 3)
                    .ToList();
                }
                else if (v.Equals("price39"))
                {
                    listings = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Where(v => v.Status == false)
                    .Include(v => v.Account)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail && v.Realty.Price >= 3 && v.Realty.Price <= 9)
                    .OrderByDescending(v => v.Realty.Price / v.Realty.Area)
                    .ToList();
                }
                else if (v.Equals("pricet9"))
                {
                    listings = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Where(v => v.Status == false)
                    .Include(v => v.Account)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail && v.Realty.Price > 9)
                    .OrderByDescending(v => v.Realty.Price / v.Realty.Area)
                    .ToList();
                }
                else if (v.Equals("aread50"))
                {
                    listings = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Where(v => v.Status == false)
                    .Include(v => v.Account)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail && v.Realty.Area < 50)
                    .OrderByDescending(v => v.Realty.Price / v.Realty.Area)
                    .ToList();
                }
                else if (v.Equals("area50200"))
                {
                    listings = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Where(v => v.Status == false)
                    .Include(v => v.Account)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail && v.Realty.Area >= 50 && v.Realty.Area <= 200)
                    .OrderByDescending(v => v.Realty.Price / v.Realty.Area)
                    .ToList();
                }
                else if (v.Equals("areat200"))
                {
                    listings = _context.Listings
                    .Where(v => v.Status == false && v.Duration == false)
                    .Where(v => v.Status == false)
                    .Include(v => v.Account)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail && v.Realty.Area > 200)
                    .OrderByDescending(v => v.Realty.Price / v.Realty.Area)
                    .ToList();
                }

            }
            else
            {
                listings = _context.Listings
                .Where(v => v.Status == false && v.Duration == false)
                .Where(v => v.Status == false)
                .Include(v => v.Account)
                .Include(v => v.Realty)
                    .ThenInclude(v => v.Images.Where(img => img.Status == false))
                .Include(v => v.Realty.CategoryDetail)
                    .ThenInclude(c => c.Category)
                .Where(v => v.Realty.CategoryDetail.CategoryDetailId == idCategoryDetail)
                .OrderBy(v => v.Location)
                //.Skip((page - 1) * pageSize)
                //.Take(pageSize)
                .ToList();
            }


            ViewBag.TieuDe = _context.CategoryDetails.Where(v => v.CategoryDetailId == idCategoryDetail).FirstOrDefault().CategoryDetailName;
            ViewBag.idcategorydetail = idCategoryDetail;

            ViewData["list_category_realties"] = _context.CategoryDetails.Where(v => v.Status == false).ToList();
            return View(listings);
        }
        [Authenticated]
        [Route("profile.html", Name = "profile")]
        public IActionResult Profile(string? mss)
        {
            var check = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "iduser");
            var account = _context.Accounts.Where(v => v.Status == false && v.AccountId == Int32.Parse(check.Value))
                .Include(v => v.Package)
                .Include(v => v.Listings)
                .FirstOrDefault();

            if (mss != null)
            {
                ViewBag.SuccessMessage = "V";
            }

            return View(account);
        }
        [Authenticated]
        [Route("package.html", Name = "package")]
        public IActionResult Package()
        {
            List<Package> packages = _context.Packages.Where(v => v.Status == false).OrderByDescending(v => v.NumberPost).ToList();
            return View(packages);
        }
        //[Authenticated]
        [Route("listing.html", Name = "listing")]
        public IActionResult Listing(int idaccount, int page, string? mss, string? mssgiahan)
        {
            int pageSize = 6;
            if (page == null || page < 1)
            {
                page = 1;
            }
            var listingOfAccount = _context.Listings
                .Where(v => v.Status == false && v.Duration == false && v.AccountId == idaccount)
                .Include(v => v.Account)
                    .ThenInclude(v => v.Package)
                .Include(v => v.Realty)
                    .ThenInclude(v => v.Images.Where(img => img.Status == false))
                .Include(v => v.Realty.CategoryDetail)
                    .ThenInclude(c => c.Category)
                .OrderBy(v => v.Location)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalPageCount = (int)Math.Ceiling((double)_context.Listings.Where(v=>v.Status==false&&v.AccountId==idaccount).Count() / pageSize);
            ViewBag.count = totalPageCount;
            if (page > totalPageCount)
            {
                page = totalPageCount;
            }
            ViewBag.accountId = idaccount;

            ViewData["account"] = _context.Accounts.Where(v => v.Status == false && v.AccountId == idaccount).FirstOrDefault();
            ViewData["listingOfAccount"] = listingOfAccount;
            ViewBag.mountListingofaccount = _context.Listings.Where(v=>v.Status==false&&v.AccountId==idaccount&&v.Duration==false).Count();

            var check = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "iduser");
            if (check != null && listingOfAccount.Count < pageSize)
            {
                listingOfAccount = _context.Listings
                    .Where(v => v.Status == false && v.Duration == true && v.AccountId == idaccount)
                    .Include(v => v.Account)
                        .ThenInclude(v => v.Package)
                    .Include(v => v.Realty)
                        .ThenInclude(v => v.Images.Where(img => img.Status == false))
                    .Include(v => v.Realty.CategoryDetail)
                        .ThenInclude(c => c.Category)
                    .OrderBy(v => v.Location)
                    //.Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                ViewData["listingOfAccountDuration"] = listingOfAccount;
            }

            ViewData["CategoryDetailId"] = new SelectList(_context.CategoryDetails.Where(v => v.Status == false), "CategoryDetailId", "CategoryDetailName");
            ViewData["RealtyId"] = new SelectList(_context.Realties.Where(v => v.Status == false), "RealtyId", "Title");
            ViewData["list_category_realties"] = _context.CategoryDetails.Where(v => v.Status == false).ToList();
            if (mss != null)
            {
                ViewBag.messcapnhat = "Cập nhật thành công!";
            }
            if (mssgiahan != null)
            {
                ViewBag.messgiahan = "Cập nhật thành công!";
            }
            return View();
        }
        public IActionResult Restore(int id)
        {
            var check = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "iduser");
            int.TryParse(check?.Value, out int iduser);
            Listing ls = _context.Listings.Where(v => v.Status == false && v.Duration == true && v.ListingId == id).FirstOrDefault();
            ls.Duration = false;
            ls.CreatedAt = DateTime.Now;
            ls.UpdatedAt = DateTime.Now;
            // trừ lượt đăng
            Account ac = _context.Accounts.Where(v => v.Status == false && v.AccountId == ls.AccountId).FirstOrDefault();
            ac.NumberPost--;

            _context.Update(ls);
            _context.SaveChanges();
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
            _context.SaveChanges();
            return RedirectToAction("Listing", new { mssgiahan = "v", idaccount = iduser });
        }
        [HttpGet]
        public async Task<IActionResult> EditListing(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }
            var check = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "iduser");
            Account account = _context.Accounts.Where(v => v.Status == false && v.AccountId == Int32.Parse(check.Value)).FirstOrDefault();
            Listing listing = await _context.Listings.FindAsync(id);
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
            ViewBag.theloai = _context.CategoryDetails.Where(v=>v.Status==false&&v.CategoryDetailId==realty.CategoryDetailId).FirstOrDefault().CategoryDetailName;
            return View(createListingViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditListing(int id, Property.ViewsModel.CreateListingViewModel model)
        {
            var check = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "iduser");
            int.TryParse(check?.Value, out int iduser);
             
            model.Listing.Status = false;
            model.Realty.Status = false;
            model.Listing.UpdatedAt = DateTime.Now;
            if (id != model.Listing.ListingId)
            {
                return RedirectToAction("Error");
            }

            if (model.Listing.Title != null && model.Listing.Description!=null && model.Realty.Title!=null&&model.Realty.Area!=null&&model.Realty.Address!=null)
            {
                try
                {
                    _context.Update(model.Listing);
                    await _context.SaveChangesAsync();

                    _context.Update(model.Realty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return RedirectToAction("Error");
                }
            }
            else
            {
                ViewBag.err = "V";
                ViewBag.theloai = _context.CategoryDetails.Where(v => v.Status == false && v.CategoryDetailId == model.Realty.CategoryDetailId).FirstOrDefault().CategoryDetailName;
                return View(model);
            }
            return RedirectToAction("Listing", new { mss = "v", idaccount = iduser });
        }
        public async Task<IActionResult> DeleteListing(int id)
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
        [Authenticated]
        public IActionResult Purchase(int id)
        {
            var check = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "iduser");
            var account = _context.Accounts.Where(v => v.Status == false && v.AccountId == Int32.Parse(check.Value)).FirstOrDefault();

            Transaction transaction = new Transaction
            {
                AccountId = account.AccountId,
                PackageId = id,
                Amount = 1,
                PaymentDate = null,
                Status = false
            };
            _context.Add(transaction);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            var account = _context.Realties.Where(v => v.Status == false).ToList();
            return View(account);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}