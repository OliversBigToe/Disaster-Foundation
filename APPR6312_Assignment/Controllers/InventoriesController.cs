using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using APPR6312_Assignment.Models;

namespace APPR6312_Assignment.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly AppDbContext _context;

        public InventoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Inventories
        public async Task<IActionResult> Index()
        {
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.name = HttpContext.Session.GetString("name");
            ViewBag.surname = HttpContext.Session.GetString("surname");

            ViewBag.disaster = HttpContext.Session.GetInt32("disaster");

            return _context.Inventory != null ?
                          View(await _context.Inventory.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Inventory'  is null.");
        }

        // GET: Inventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Inventory == null)
            {
                return NotFound();
            }

            var inventories = await _context.Inventory
                .FirstOrDefaultAsync(m => m.invID == id);
            if (inventories == null)
            {
                return NotFound();
            }

            return View(inventories);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("PublicPage", "Disasters");
        }

        // GET: Inventories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int invAmount, string invCategory, decimal transAmount)
        {
            var totalMoney = _context.Money.Sum(x => x.moneyAmount);
            ViewBag.totalMoney = totalMoney;

            var remainingMoney = _context.Transactions.Sum(x => x.transAmount);
            ViewBag.remainingMoney = remainingMoney;

            Inventories inv = new Inventories();

            //Amount cannot be lower than 0
            if (remainingMoney < transAmount)
            {
                ViewBag.error = "Insufficent funds!";
                return View(inv);
            }

            inv.invCategory = invCategory;
            inv.invAmount = invAmount;
            inv.invDate = DateTime.Now;

            _context.Inventory.Add(inv);

            Transaction trans = new Transaction();
            trans.transDate = DateTime.Now;
            trans.transAmount = transAmount * -1;
            trans.transType = "Purchased Goods";

            _context.Transactions.Add(trans);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Inventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Inventory == null)
            {
                return NotFound();
            }

            var inventories = await _context.Inventory.FindAsync(id);
            if (inventories == null)
            {
                return NotFound();
            }
            return View(inventories);
        }

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("invID,invDate,invAmount,invCategory")] Inventories inventories)
        {
            if (id != inventories.invID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventories);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoriesExists(inventories.invID))
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
            return View(inventories);
        }

        // GET: Inventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Inventory == null)
            {
                return NotFound();
            }

            var inventories = await _context.Inventory
                .FirstOrDefaultAsync(m => m.invID == id);
            if (inventories == null)
            {
                return NotFound();
            }

            return View(inventories);
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Inventory == null)
            {
                return Problem("Entity set 'AppDbContext.Inventory'  is null.");
            }
            var inventories = await _context.Inventory.FindAsync(id);
            if (inventories != null)
            {
                _context.Inventory.Remove(inventories);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoriesExists(int id)
        {
            return _context.Inventory.Any(e => e.invID == id);
        }
    }
}
