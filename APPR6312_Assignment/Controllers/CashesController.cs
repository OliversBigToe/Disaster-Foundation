using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using APPR6312_Assignment.Models;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using System.Security.Policy;

namespace APPR6312_Assignment.Controllers
{
    public class CashesController : Controller
    {
        private readonly AppDbContext _context;

        public CashesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Cashes
        public async Task<IActionResult> Index()
        {
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.name = HttpContext.Session.GetString("name");
            ViewBag.surname = HttpContext.Session.GetString("surname");

            ViewBag.disaster = HttpContext.Session.GetInt32("disaster");

            return _context.Money != null ?
                          View(await _context.Money.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Money'  is null.");
        }

        // GET: Cashes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Money == null)
            {
                return NotFound();
            }

            var cash = await _context.Money
                .FirstOrDefaultAsync(m => m.moneyID == id);
            if (cash == null)
            {
                return NotFound();
            }

            return View(cash);
        }

        // GET: Cashes/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult AllocateMoney()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateMoney(decimal amount, int id)
        {
            var totalMoney = _context.Money.Sum(x => x.moneyAmount);
            ViewBag.totalMoney = totalMoney;

            var remainingMoney = _context.Transactions.Sum(x => x.transAmount);
            ViewBag.remainingMoney = remainingMoney;

            Transaction trans = new Transaction();

            //Amount cannot be lower than 0
            if (remainingMoney < amount)
            {
                ViewBag.error = "Insufficent funds!";
                return View(trans);
            }

            //Task 2 Writting to Transactions Table
            trans.transDate = DateTime.Now;
            trans.transAmount = amount * -1;
            trans.transType = "Allocation";
            _context.Transactions.Add(trans);
            await _context.SaveChangesAsync();

            //LINQ to locate and update the allocatedMoney in the Disaster table when an allocation is made
            Disasters dID = _context.Disaster.FirstOrDefault(x => x.disasterID == id);
            HttpContext.Session.SetInt32("disaster", dID.disasterID);
            ViewBag.disaster = HttpContext.Session.GetInt32("disaster");
            dID.allocatedMoney = dID.allocatedMoney + amount;
            _context.Disaster.Update(dID);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Disasters");
        }

        // POST: Cashes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("moneyID,moneyDate,moneyAmount,goodsDonor")] Cash cash)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cash);

                //Task 2 Writting to Transactions Table
                Transaction trans = new Transaction();
                trans.transDate = cash.moneyDate;
                trans.transAmount = cash.moneyAmount;
                trans.transType = "Donation";
                _context.Add(trans);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cash);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("PublicPage", "Disasters");
        }

        // GET: Cashes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Money == null)
            {
                return NotFound();
            }

            var cash = await _context.Money.FindAsync(id);
            if (cash == null)
            {
                return NotFound();
            }
            return View(cash);
        }

        // POST: Cashes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("moneyID,moneyDate,moneyAmount,goodsDonor")] Cash cash)
        {
            if (id != cash.moneyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cash);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CashExists(cash.moneyID))
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
            return View(cash);
        }

        // GET: Cashes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Money == null)
            {
                return NotFound();
            }

            var cash = await _context.Money
                .FirstOrDefaultAsync(m => m.moneyID == id);
            if (cash == null)
            {
                return NotFound();
            }

            return View(cash);
        }

        // POST: Cashes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Money == null)
            {
                return Problem("Entity set 'Cash_Context.Money'  is null.");
            }
            var cash = await _context.Money.FindAsync(id);
            if (cash != null)
            {
                _context.Money.Remove(cash);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CashExists(int id)
        {
            return (_context.Money?.Any(e => e.moneyID == id)).GetValueOrDefault();
        }
    }
}

