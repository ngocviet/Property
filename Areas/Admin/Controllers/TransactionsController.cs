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
    public class TransactionsController : Controller
    {
        private readonly ProtalContext _context;

        public TransactionsController(ProtalContext context)
        {
            _context = context;
        }

        // GET: Admin/Transactions
        public async Task<IActionResult> Index()
        {
            var protalContext = _context.Transactions.Where(v=>v.Status == false && v.PaymentDate == null).Include(t => t.Account).Include(t => t.Package);
            ViewData["list_confirm"] = _context.Transactions.Where(v=>v.Status == false && v.PaymentDate != null).Include(t => t.Account).Include(t => t.Package).ToList();
            return View(await protalContext.ToListAsync());
        }

        // GET: Admin/Transactions/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Transactions == null)
        //    {
        //        return NotFound();
        //    }

        //    var transaction = await _context.Transactions
        //        .Include(t => t.Account)
        //        .Include(t => t.Package)
        //        .FirstOrDefaultAsync(m => m.TransactionId == id);
        //    if (transaction == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(transaction);
        //}
        public IActionResult Confirm(int id)
        {
            Transaction transaction = _context.Transactions.Where(v => v.TransactionId == id && v.Status == false).FirstOrDefault();
            if (transaction!=null)
            {
                transaction.PaymentDate = DateTime.Now;
                _context.Update(transaction);
                _context.SaveChanges();

                Account account = _context.Accounts.Where(v => v.AccountId == transaction.AccountId && v.Status == false).FirstOrDefault();
                Package package = _context.Packages.Where(v => v.PackageId == transaction.PackageId && v.Status == false).FirstOrDefault();
                account.NumberPost += package.NumberPost;
                _context.Update(account);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Admin/Transactions/Create
        //public IActionResult Create()
        //{
        //    ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "Name");
        //    ViewData["PackageId"] = new SelectList(_context.Packages, "PackageId", "PackageName");
        //    return View();
        //}

        // POST: Admin/Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("TransactionId,AccountId,PackageId,Amount,PaymentDate,Status")] Transaction transaction)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(transaction);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "Name", transaction.AccountId);
        //    ViewData["PackageId"] = new SelectList(_context.Packages, "PackageId", "PackageName", transaction.PackageId);
        //    return View(transaction);
        //}

        // GET: Admin/Transactions/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Transactions == null)
        //    {
        //        return NotFound();
        //    }

        //    var transaction = await _context.Transactions.FindAsync(id);
        //    if (transaction == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "Name", transaction.AccountId);
        //    ViewData["PackageId"] = new SelectList(_context.Packages, "PackageId", "PackageName", transaction.PackageId);
        //    return View(transaction);
        //}

        // POST: Admin/Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("TransactionId,AccountId,PackageId,Amount,PaymentDate,Status")] Transaction transaction)
        //{
        //    transaction.Status = false;
        //    if (id != transaction.TransactionId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(transaction);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!TransactionExists(transaction.TransactionId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "Name", transaction.AccountId);
        //    ViewData["PackageId"] = new SelectList(_context.Packages, "PackageId", "PackageName", transaction.PackageId);
        //    return View(transaction);
        //}

        // GET: Admin/Transactions/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'ProtalContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                transaction.Status = true;
                _context.Update(transaction);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
          return (_context.Transactions?.Any(e => e.TransactionId == id)).GetValueOrDefault();
        }
    }
}
