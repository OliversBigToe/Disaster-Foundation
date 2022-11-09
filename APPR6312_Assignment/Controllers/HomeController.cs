using APPR6312_Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace APPR6312_Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _user;

        public HomeController(AppDbContext user)
        {
            _user = user;
        }


        public IActionResult Index()
        {
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.name = HttpContext.Session.GetString("name");
            ViewBag.surname = HttpContext.Session.GetString("surname");

            ViewBag.disaster = HttpContext.Session.GetString("disaster");

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("PublicPage", "Disasters");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AdminPanel()
        {
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.name = HttpContext.Session.GetString("name");
            ViewBag.surname = HttpContext.Session.GetString("surname");

            ViewBag.disaster = HttpContext.Session.GetString("disaster");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}