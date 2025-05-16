using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Models;

namespace Project.Controllers
{
    public class UserDashboardController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: UserDashboard/Dashboard
        [HttpGet]
        public ActionResult Dashboard()
        {
            if (Session["Role"]?.ToString() != "member")
                return RedirectToAction("Index", "Login");



            int userId = (int)Session["UserId"];

            // Fetch user-specific stats
            ViewBag.MyBorrowedBooks = db.borrowing_record.Count(b => b.user_id == userId && b.return_timestamp == null);

            // FIXED: Sum fines only for the user's borrowings
            var userBorrowingIds = db.borrowing_record
                                      .Where(b => b.user_id == userId)
                                      .Select(b => b.borrowing_id)
                                      .ToList();

            ViewBag.MyFines = db.fines
                                .Where(f => userBorrowingIds.Contains((int)f.borrowing_id))
                                .Sum(f => (decimal?)f.fine_amount) ?? 0;

            ViewBag.MyReservations = db.reservations.Count(r => r.user_id == userId);
            ViewBag.MyHistory = db.borrowing_record.Count(b => b.user_id == userId);

            return View();
        }
    }
}