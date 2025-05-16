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
    public class booksController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: books
        public ActionResult Index()
        {
            var books = db.books.Include(b => b.author).Include(b => b.genre).Include(b => b.publisher);
            return View(books.ToList());
        }

        // GET: books/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: books/Create
        public ActionResult Create()
        {
            ViewBag.author_id = new SelectList(db.authors, "author_id", "name");
            ViewBag.genre_name = new SelectList(db.genres, "genre_name", "genre_description");
            ViewBag.publisher_id = new SelectList(db.publishers, "publisher_id", "name");
            return View();
        }

        // POST: books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ISBN,title,author_id,publisher_id,genre_name,totalCopies,borrowedCopies,avlCopies,publication_year")] book book)
        {
            if (ModelState.IsValid)
            {
                db.books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.author_id = new SelectList(db.authors, "author_id", "name", book.author_id);
            ViewBag.genre_name = new SelectList(db.genres, "genre_name", "genre_description", book.genre_name);
            ViewBag.publisher_id = new SelectList(db.publishers, "publisher_id", "name", book.publisher_id);
            return View(book);
        }

        // GET: books/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.author_id = new SelectList(db.authors, "author_id", "name", book.author_id);
            ViewBag.genre_name = new SelectList(db.genres, "genre_name", "genre_description", book.genre_name);
            ViewBag.publisher_id = new SelectList(db.publishers, "publisher_id", "name", book.publisher_id);
            return View(book);
        }

        // POST: books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ISBN,title,author_id,publisher_id,genre_name,totalCopies,borrowedCopies,avlCopies,publication_year")] book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.author_id = new SelectList(db.authors, "author_id", "name", book.author_id);
            ViewBag.genre_name = new SelectList(db.genres, "genre_name", "genre_description", book.genre_name);
            ViewBag.publisher_id = new SelectList(db.publishers, "publisher_id", "name", book.publisher_id);
            return View(book);
        }

        // GET: books/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            book book = db.books.Find(id);
            db.books.Remove(book);
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
