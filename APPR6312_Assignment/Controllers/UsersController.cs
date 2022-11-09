using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using APPR6312_Assignment.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace APPR6312_Assignment.Controllers
{
    public class UsersController : Controller
    {
        private AppDbContext _context;

        User user = new User();

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.name = HttpContext.Session.GetString("name");
            ViewBag.surname = HttpContext.Session.GetString("surname");

            ViewBag.disaster = HttpContext.Session.GetInt32("disaster");

            var email = HttpContext.Session.GetString("email");

            if (String.IsNullOrWhiteSpace(email))
            {
                return _context.Users != null ?
                              View(await _context.Users.ToListAsync()) :
                              Problem("Entity set 'User_Context.Users'  is null.");
            }

            return _context.Users != null ?
              View(await _context.Users.Where(x => x.userEmail == email).ToListAsync()) :
              Problem("Entity set 'User_Context.Users'  is null.");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("PublicPage", "Disasters");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.userEmail == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        //GET
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Approve(string? email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var u = _context.Users.Find(email);

                u.userRole = "Approved";

                _context.Update(u);
                _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public ActionResult Deny(string? email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var u = _context.Users.Find(email);

                u.userRole = "Denied";

                _context.Update(u);
                _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageUsers");
        }


        public async Task<IActionResult> ManageUsers(User user)
        {
            var details = _context.Users.FirstOrDefault(x => x.userEmail == user.userEmail);

            return _context.Users != null ?
                        View(await _context.Users.Where(x => x.userRole != "Admin").ToListAsync()) :
                        Problem("Entity set 'User_Context.Users'  is null.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string userEmail, string userPassword)
        {
            if (ModelState.IsValid)
            {
                var details = _context.Users.FirstOrDefault(x => x.userEmail == userEmail);
                var e_password = GetMD5(userPassword);
                var data = _context.Users.Where(s => s.userEmail.Equals(userEmail) && s.userPassword.Equals(e_password)).ToList();

                var vAdmin = _context.Users.Where(a => a.userEmail == userEmail && a.userPassword == e_password && a.userRole == "Admin").FirstOrDefault();
                var vUser = _context.Users.Where(u => u.userEmail == userEmail && u.userPassword == e_password && u.userRole == "Pending").FirstOrDefault();
                var vApproved = _context.Users.Where(u => u.userEmail == userEmail && u.userPassword == e_password && u.userRole == "Approved").FirstOrDefault();
                var vDenied = _context.Users.Where(u => u.userEmail == userEmail && u.userPassword == e_password && u.userRole == "Denied").FirstOrDefault();



                if (details == null)
                {
                    return Login();
                }
                if (userPassword == null)
                {
                    return Login();
                }
                if (details.userPassword != e_password)
                {
                    return Login();
                }

                if (vAdmin != null)
                {
                    HttpContext.Session.SetString("email", details.userEmail);
                    ViewBag.email = HttpContext.Session.GetString("email");
                    HttpContext.Session.SetString("name", details.userName);
                    ViewBag.name = HttpContext.Session.GetString("name");
                    HttpContext.Session.SetString("surname", details.userSurname);
                    ViewBag.surname = HttpContext.Session.GetString("surname");
                    HttpContext.Session.SetString("role", details.userRole);
                    ViewBag.role = HttpContext.Session.GetString("role");

                    return RedirectToAction("AdminPanel", "Home");
                }
                else
                {
                    if (vApproved != null)
                    {
                        HttpContext.Session.SetString("email", details.userEmail);
                        ViewBag.email = HttpContext.Session.GetString("email");
                        HttpContext.Session.SetString("name", details.userName);
                        ViewBag.name = HttpContext.Session.GetString("name");
                        HttpContext.Session.SetString("surname", details.userSurname);
                        ViewBag.surname = HttpContext.Session.GetString("surname");
                        HttpContext.Session.SetString("role", details.userRole);
                        ViewBag.role = HttpContext.Session.GetString("role");

                        return RedirectToAction("Index", "Home");
                    }
                }

                if (vDenied != null)
                {
                    return Login();
                }
            }
            return View();
        }

        //GET
        public IActionResult Register()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var check = _context.Users.FirstOrDefault(s => s.userEmail == user.userEmail);
                if (check == null)
                {
                    user.userPassword = GetMD5(user.userPassword);
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }
            }

            return View(user);

        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("userEmail,userPassword,userName,userSurname,userRole")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("userEmail,userPassword,userName,userSurname,userRole")] User user)
        {
            if (id != user.userEmail)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.userPassword = GetMD5(user.userPassword);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.userEmail))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.userEmail == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'AppDbContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.userEmail == id)).GetValueOrDefault();
        }
    }
}
