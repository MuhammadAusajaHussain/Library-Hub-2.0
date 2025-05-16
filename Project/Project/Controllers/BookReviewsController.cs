// Controllers/BookReviewsController.cs
using System;
using System.Linq;
using System.Web.Mvc;
using Project.Models;
using System.Data.Entity;

namespace Project.Controllers
{
    public class BookReviewsController : Controller
    {
        private readonly LMSEntities db = new LMSEntities();

        // GET: /BookReviews/Index
        public ActionResult Index()
        {
            var reviews = db.book_reviews
                .Include(r => r.book)
                .Include(r => r.user)
                .OrderByDescending(r => r.review_timestamp)
                .Select(r => new BookReviewListViewModel
                {
                    BookTitle = r.book.title,
                    UserName = r.user.user_name,
                    Rating = (int)r.rating,
                    Comment = r.comment,
                    ReviewTimestamp = (DateTime)r.review_timestamp
                })
                .ToList();

            return View(reviews);
        }

        // GET: /BookReviews/Create
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "Login");

            ViewBag.ISBN = new SelectList(db.books, "ISBN", "title");
            return View();
        }

        // POST: /BookReviews/Create
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(string ISBN, int rating, string comment)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "Login");

            if (string.IsNullOrEmpty(ISBN) || rating < 1 || rating > 5)
            {
                ModelState.AddModelError("", "Please select a book and valid rating (1–5).");
            }

            if (ModelState.IsValid)
            {
                int nextId = db.book_reviews.Any()
                    ? db.book_reviews.Max(r => r.review_id) + 1
                    : 1;

                db.book_reviews.Add(new book_reviews
                {
                    review_id = nextId,
                    ISBN = ISBN,
                    user_id = (int)Session["UserId"],
                    rating = rating,
                    comment = comment,
                    review_timestamp = DateTime.Now
                });

                db.SaveChanges();
                TempData["Success"] = "Review submitted.";
                return RedirectToAction("Index");
            }

            ViewBag.ISBN = new SelectList(db.books, "ISBN", "title", ISBN);
            return View();
        }
    }

    public class BookReviewListViewModel
    {
        public string BookTitle { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewTimestamp { get; set; }
    }
}
