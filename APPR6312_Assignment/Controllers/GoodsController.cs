using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using APPR6312_Assignment.Models;
using System.Security.Policy;
using System.Diagnostics;

namespace APPR6312_Assignment.Controllers
{
    public class GoodsController : Controller
    {
        private readonly AppDbContext _context;

        public GoodsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Goods
        public async Task<IActionResult> Index()
        {
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.name = HttpContext.Session.GetString("name");
            ViewBag.surname = HttpContext.Session.GetString("surname");

            ViewBag.disaster = HttpContext.Session.GetString("disaster");

            return _context.Goods != null ? 
                          View(await _context.Goods.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Goods'  is null.");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("PublicPage", "Disasters");
        }

        // GET: Goods/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Goods == null)
            {
                return NotFound();
            }

            var good = await _context.Goods
                .FirstOrDefaultAsync(m => m.goodsID == id);
            if (good == null)
            {
                return NotFound();
            }

            return View(good);
        }

        // GET: Goods/Create
        public IActionResult Create()
        {
            var outputList = _context.Goods.Select(x => x.goodsCategory).Distinct().ToList();
            ViewData["Categories"] = outputList;

            return View();
        }

        public IActionResult AllocateGoods()
        {
            var outputList = _context.Inventory.Select(x => x.invCategory).Distinct().ToList();
            ViewData["Categories"] = outputList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateGoods(int amount, string category, int id)
        {
            Inventories inv = new Inventories();

            var totalGoods = _context.Goods.Sum(x => x.goodsAmount);
            ViewBag.totalGoods = totalGoods;

            var remainingGoods = _context.Inventory.Sum(x => x.invAmount);
            ViewBag.remainingGoods = remainingGoods;

            var outputList = _context.Inventory.Select(x => x.invCategory).Distinct().ToList();
            ViewData["Categories"] = outputList;

            //Amount cannot be lower than 0
            if (remainingGoods < amount)
            {
                ViewBag.error = "Insufficent goods!";
                return View(inv);
            }

            Disasters dID = _context.Disaster.FirstOrDefault(x => x.disasterID == id);

            HttpContext.Session.SetString("disasterName", dID.disasterName);
            ViewBag.disasterName = HttpContext.Session.GetString("disasterName");

            //Task 2 Writting to Inventory Table
            inv.invDate = DateTime.Now;
            inv.invAmount = amount * -1;
            inv.invCategory = category;
            _context.Inventory.Add(inv);
            await _context.SaveChangesAsync();

            //LINQ to locate and update the allocatedGoods and goodsCategory in the Disaster table when an allocation is made
            HttpContext.Session.SetInt32("disaster", dID.disasterID);
            ViewBag.disaster = HttpContext.Session.GetInt32("disaster");
            dID.allocatedGoods = dID.allocatedGoods + amount;
            dID.goodsCategory = category;
            _context.Disaster.Update(dID);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Disasters");
        }


        // POST: Goods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("goodsID,goodsDate,goodsAmount,goodsDescription,goodsCategory,goodsDonor")] Good good)
        {
            if (ModelState.IsValid)
            {
                _context.Add(good);

                //Task 2 Writting to Inventory Table
                Inventories inv = new Inventories();
                inv.invDate = good.goodsDate;
                inv.invAmount = good.goodsAmount;
                inv.invCategory = good.goodsCategory;
                _context.Add(inv);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(good);
        }

        // GET: Goods/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Goods == null)
            {
                return NotFound();
            }

            var good = await _context.Goods.FindAsync(id);
            if (good == null)
            {
                return NotFound();
            }
            return View(good);
        }

        // POST: Goods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("goodsID,goodsDate,goodsAmount,goodsDescription,goodsCategory,goodsDonor")] Good good)
        {
            if (id != good.goodsID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(good);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoodExists(good.goodsID))
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
            return View(good);
        }

        // GET: Goods/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Goods == null)
            {
                return NotFound();
            }

            var good = await _context.Goods
                .FirstOrDefaultAsync(m => m.goodsID == id);
            if (good == null)
            {
                return NotFound();
            }

            return View(good);
        }

        // POST: Goods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Goods == null)
            {
                return Problem("Entity set 'Good_Context.Goods'  is null.");
            }
            var good = await _context.Goods.FindAsync(id);
            if (good != null)
            {
                _context.Goods.Remove(good);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoodExists(int id)
        {
          return (_context.Goods?.Any(e => e.goodsID == id)).GetValueOrDefault();
        }
    }
}
