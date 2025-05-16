using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project.Models;

namespace Project.Controllers
{
    public class book_reviewsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: book_reviews
        public ActionResult Index()
        {
            var book_reviews = db.book_reviews.Include(b => b.book).Include(b => b.user);
            return View(book_reviews.ToList());
        }

        // GET: book_reviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book_reviews book_reviews = db.book_reviews.Find(id);
            if (book_reviews == null)
            {
                return HttpNotFound();
            }
            return View(book_reviews);
        }

        // GET: book_reviews/Create
        public ActionResult Create()
        {
            ViewBag.ISBN = new SelectList(db.books, "ISBN", "title");
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name");
            return View();
        }

        // POST: book_reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ISBN,user_id,rating,comment,review_timestamp")] book_reviews book_reviews)
        {
            if (ModelState.IsValid)
            {
                // Get the last review_id and add 1
                int lastId = db.book_reviews.Any()
                    ? db.book_reviews.Max(r => r.review_id)
                    : 0;

                book_reviews.review_id = lastId + 1;

                db.book_reviews.Add(book_reviews);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ISBN = new SelectList(db.books, "ISBN", "title", book_reviews.ISBN);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", book_reviews.user_id);
            return View(book_reviews);
        }


        // GET: book_reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book_reviews book_reviews = db.book_reviews.Find(id);
            if (book_reviews == null)
            {
                return HttpNotFound();
            }
            ViewBag.ISBN = new SelectList(db.books, "ISBN", "title", book_reviews.ISBN);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", book_reviews.user_id);
            return View(book_reviews);
        }

        // POST: book_reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "review_id,ISBN,user_id,rating,comment,review_timestamp")] book_reviews book_reviews)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book_reviews).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ISBN = new SelectList(db.books, "ISBN", "title", book_reviews.ISBN);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", book_reviews.user_id);
            return View(book_reviews);
        }

        // GET: book_reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book_reviews book_reviews = db.book_reviews.Find(id);
            if (book_reviews == null)
            {
                return HttpNotFound();
            }
            return View(book_reviews);
        }

        // POST: book_reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            book_reviews book_reviews = db.book_reviews.Find(id);
            db.book_reviews.Remove(book_reviews);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
