using System;
using System.Linq;
using System.Web.Mvc;
using Project.Models;

namespace Project.Controllers
{
    public class BorrowController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Borrow
        public ActionResult Index(string search)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "Login");

            var books = db.books
                .Where(b => b.avlCopies > 0 &&
                            (string.IsNullOrEmpty(search) ||
                             b.title.Contains(search) ||
                             b.author.name.Contains(search)))
                .ToList();

            ViewBag.Search = search;
            return View(books);
        }

        // POST: Borrow
        [HttpPost]
        public ActionResult BorrowBook(string ISBN)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "Login");

            int userId = (int)Session["UserId"];
            var book = db.books.FirstOrDefault(b => b.ISBN == ISBN);

            if (book != null && book.avlCopies > 0)
            {
                // Manually generate next borrowing_id
                int nextId = 1;
                if (db.borrowing_record.Any())
                {
                    nextId = db.borrowing_record.Max(b => b.borrowing_id) + 1;
                }

                // Create new borrowing record
                var borrow = new borrowing_record
                {
                    borrowing_id = nextId,
                    user_id = userId,
                    ISBN = ISBN,
                    borrow_timestamp = DateTime.Now,
                    due_timestamp = DateTime.Now.AddDays(3),
                    status = "borrowed"
                };

                db.borrowing_record.Add(borrow);

                // Update book's borrowedCopies
                book.borrowedCopies += 1;

                db.SaveChanges();
                TempData["Success"] = "Book borrowed successfully.";
            }
            else
            {
                TempData["Error"] = "Book is not available.";
            }

            return RedirectToAction("Index");
        }



        // GET: BorrowedBooks
        public ActionResult BorrowedBooks()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "Login");

            int userId = (int)Session["UserId"];

            var borrowed = db.borrowing_record
                .Where(b => b.user_id == userId && b.status == "borrowed")
                .ToList();

            return View(borrowed);
        }

        // POST: ReturnBook
        [HttpPost]
        public ActionResult ReturnBook(int borrowing_id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "Login");

            var record = db.borrowing_record.FirstOrDefault(b => b.borrowing_id == borrowing_id);

            if (record != null && record.status == "borrowed")
            {
                record.status = "returned";
                record.return_timestamp = DateTime.Now;

                var book = db.books.FirstOrDefault(b => b.ISBN == record.ISBN);
                if (book != null && book.borrowedCopies > 0)
                {
                    book.borrowedCopies -= 1;
                }

                db.SaveChanges();
                TempData["Success"] = "Book returned successfully.";
            }

            return RedirectToAction("BorrowedBooks");
        }

    }
}
