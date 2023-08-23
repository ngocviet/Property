using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Property.Models;

namespace Property.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly ProtalContext _context;
        private readonly IWebHostEnvironment v;

        public AccountsController(ProtalContext context, IWebHostEnvironment v)
        {
            _context = context;
            this.v = v;
        }

        // GET: Admin/Accounts
        public async Task<IActionResult> Index()
        {
            List<Account> protalContext = _context.Accounts.Where(v => v.Status == false)
                .Include(a => a.Package)
                .Include(a => a.Listings.Where(v => v.Status == false)).ToList();
            return View(protalContext);
        }

        // GET: Admin/Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            Account account = await _context.Accounts
                .Include(a => a.Package)
                .FirstOrDefaultAsync(m => m.AccountId == id);

            ViewBag.sobaidang = _context.Listings.Where(v => v.Status == false && v.AccountId == id).Count();
            ViewBag.sobaihethan = _context.Listings.Where(v => v.Status == true && v.AccountId == id).Count();

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }
        string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        // GET: Admin/Accounts/Create
        public IActionResult Create()
        {
            ViewData["PackageId"] = new SelectList(_context.Packages, "PackageId", "PackageName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,PackageId,Username,Password,Name,Address,Phone,Avatar,Permissions,Status")] Account account, IFormFile img)
        {
            if (account.Avatar == null)
            {
                account.Avatar = "noimg.gif";
            }
            if (ModelState.IsValid || account.Avatar != null)
            {
                if (account.PackageId == null || account.Username == null || account.Password == null || account.Name == null || account.Address == null || account.Phone == null || account.Permissions == null)
                {
                    ViewData["PackageId"] = new SelectList(_context.Packages, "PackageId", "PackageName", account.PackageId);
                    return View(account);
                }
                List<Account> listAcc = _context.Accounts.Where(v => v.Status == false).ToList();
                foreach (var item in listAcc)
                {
                    if (item.Username.Equals(account.Username))
                    {
                        ViewBag.tktontai = "Tài khoản đã tồn tại.";
                        ViewData["PackageId"] = new SelectList(_context.Packages, "PackageId", "PackageName", account.PackageId);
                        return View(account);
                    }
                }
                if (img != null && img.Length > 0)
                {
                    string uploadsFolder = Path.Combine(v.WebRootPath, "images");
                    string uniqueFileName = /*Guid.NewGuid().ToString() + "_" +*/ img.FileName; // Sử dụng Guid để tạo tên file duy nhất
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(fileStream);
                    }
                    account.Avatar = uniqueFileName;
                }
                account.NumberPost = _context.Packages.Where(v => v.PackageId == account.PackageId).FirstOrDefault().NumberPost;
                account.Password = GetMd5Hash(account.Password);
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PackageId"] = new SelectList(_context.Packages, "PackageId", "PackageName", account.PackageId);
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["PackageId"] = new SelectList(_context.Packages, "PackageId", "PackageName", account.PackageId);
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,PackageId,Username,Password,Name,Address,Phone,Avatar,Permissions,Status,NumberPost")] Account account, IFormFile img)
        {
            account.Status = false;
            if (id != account.AccountId)
            {
                return NotFound();
            }
            if (account.Avatar == null)
            {
                account.Avatar = "noimg.gif";
            }
            if (ModelState.IsValid || account.Avatar != null)
            {
                if (account.PackageId == null || account.Username == null || account.Password == null || account.Name == null || account.Address == null || account.Phone == null || account.Permissions == null)
                {
                    ViewData["PackageId"] = new SelectList(_context.Packages, "PackageId", "PackageName", account.PackageId);
                    return View(account);
                }
                if (img != null && img.Length > 0)
                {
                    string uploadsFolder = Path.Combine(v.WebRootPath, "images");
                    string uniqueFileName = /*Guid.NewGuid().ToString() + "_" +*/ img.FileName; // Sử dụng Guid để tạo tên file duy nhất
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(fileStream);
                    }
                    account.Avatar = uniqueFileName;
                }
                try
                {
                    account.Password = GetMd5Hash(account.Password);
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
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
            ViewData["PackageId"] = new SelectList(_context.Packages, "PackageId", "PackageName", account.PackageId);
            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set 'ProtalContext.Accounts'  is null.");
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                List<Listing> lis = _context.Listings.Where(v => v.AccountId == id).ToList();
                foreach (Listing li in lis)
                {
                    li.Status = true;
                }
                if (lis.Count > 0)
                {
                    Realty real = _context.Realties.Where(v => v.RealtyId == lis.FirstOrDefault().RealtyId).FirstOrDefault();
                    real.Status = true;
                }

                account.Status = true;
                _context.Update(account);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return (_context.Accounts?.Any(e => e.AccountId == id)).GetValueOrDefault();
        }
    }
}
