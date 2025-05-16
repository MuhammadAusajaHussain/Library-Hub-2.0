using System;
using System.Linq;
using System.Web.Mvc;
using Project.Models;

namespace Project.Controllers
{
    public class AdminDashboardController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: AdminDashboard/Dashboard
        [HttpGet]
        public ActionResult Dashboard()
        {
            // Ensure only admin can access this page
            if (Session["Role"]?.ToString() != "admin")
                return RedirectToAction("Index", "Login");

            // Fetch admin stats
            ViewBag.TotalUsers = db.users.Count();
            ViewBag.TotalBooks = db.books.Count();
            ViewBag.TotalBorrowedBooks = db.borrowing_record.Count(b => b.return_timestamp == null);
            ViewBag.TotalFines = db.fines.Sum(f => (decimal?)f.fine_amount) ?? 0;
            ViewBag.TotalGenres = db.genres.Count();
            ViewBag.TotalPublishers = db.publishers.Count();

            return View();
        }
    }
}
