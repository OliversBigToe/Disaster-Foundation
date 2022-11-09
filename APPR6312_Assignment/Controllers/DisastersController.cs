using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using APPR6312_Assignment.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace APPR6312_Assignment.Controllers
{
    public class DisastersController : Controller
    {
        private readonly AppDbContext _context;

        public DisastersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Disasters
        public async Task<IActionResult> Index()
        {
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.name = HttpContext.Session.GetString("name");
            ViewBag.surname = HttpContext.Session.GetString("surname");

            ViewBag.disaster = HttpContext.Session.GetString("disaster");

            ViewBag.totalAllocations = _context.Disaster.Sum(s => s.allocatedMoney);

            var totalMoney = _context.Money.Sum(x => x.moneyAmount);
            ViewBag.totalMoney = totalMoney;

            var remainingMoney = _context.Transactions.Sum(x => x.transAmount);
            ViewBag.remainingMoney = remainingMoney;

            return _context.Disaster != null ? 
                          View(await _context.Disaster.ToListAsync()) :
                          Problem("Entity set 'Disasters_Context.Disaster'  is null.");


        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("PublicPage", "Disasters");
        }

        [HttpGet]
        public async Task<IActionResult> PublicPage()
        {
            var disasters = _context.Disaster.ToList();

            //Getting the remaining amount of available money 
            var remainingMoney = _context.Transactions.Sum(x => x.transAmount);
            ViewBag.remainingMoney = remainingMoney;

            //Getting the total amount of money donated
            var totalMoney = _context.Money.Sum(x => x.moneyAmount);
            ViewBag.totalMoney = totalMoney;

            //Getting the total amount of goods donated
            var totalGoods = _context.Goods.Sum(x => x.goodsAmount);
            ViewBag.totalGoods = totalGoods;

            //Getting the remaining of goods in inventory
            var remainingGoods = _context.Inventory.Sum(x => x.invAmount);
            ViewBag.remainingGoods = remainingGoods;

            return View(disasters);
        }


        // GET: Disasters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Disaster == null)
            {
                return NotFound();
            }

            var disasters = await _context.Disaster
                .FirstOrDefaultAsync(m => m.disasterID == id);
            if (disasters == null)
            {
                return NotFound();
            }

            return View(disasters);
        }

        // GET: Disasters/Create
        public IActionResult Create()
        {
            var outputList = _context.Disaster.Select(x => x.aidType).Distinct().ToList();
            ViewData["Aids"] = outputList;

            return View();
        }

        // POST: Disasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("disasterID,disasterName,disasterStartDate,disasterEndDate,disasterLocation,aidType,disasterDescription,goodsCategory")] Disasters disasters)
        {
            var outputList = _context.Disaster.Select(x => x.aidType).Distinct().ToList();
            ViewData["Aids"] = outputList;

            if (ModelState.IsValid)
            {
                _context.Add(disasters);
                disasters.allocatedMoney = 0;
                disasters.allocatedGoods = 0;
                disasters.goodsCategory = "Nothing... yet";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(disasters);
        }

        // GET: Disasters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Disaster == null)
            {
                return NotFound();
            }

            var disasters = await _context.Disaster.FindAsync(id);
            if (disasters == null)
            {
                return NotFound();
            }
            return View(disasters);
        }

        // POST: Disasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("disasterID,disasterName,disasterStartDate,disasterEndDate,disasterLocation,aidType,disasterDescription")] Disasters disasters)
        {
            if (id != disasters.disasterID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(disasters);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisastersExists(disasters.disasterID))
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
            return View(disasters);
        }

        // GET: Disasters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Disaster == null)
            {
                return NotFound();
            }

            var disasters = await _context.Disaster
                .FirstOrDefaultAsync(m => m.disasterID == id);
            if (disasters == null)
            {
                return NotFound();
            }

            return View(disasters);
        }

        // POST: Disasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Disaster == null)
            {
                return Problem("Entity set 'AppDbContext.Disaster'  is null.");
            }
            var disasters = await _context.Disaster.FindAsync(id);
            if (disasters != null)
            {
                _context.Disaster.Remove(disasters);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DisastersExists(int id)
        {
          return (_context.Disaster?.Any(e => e.disasterID == id)).GetValueOrDefault();
        }
    }
}
