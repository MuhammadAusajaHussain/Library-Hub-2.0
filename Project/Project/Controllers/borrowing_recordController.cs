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
    public class borrowing_recordController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: borrowing_record
        public ActionResult Index()
        {
            var borrowing_record = db.borrowing_record.Include(b => b.book).Include(b => b.user);
            return View(borrowing_record.ToList());
        }

        // GET: borrowing_record/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrowing_record borrowing_record = db.borrowing_record.Find(id);
            if (borrowing_record == null)
            {
                return HttpNotFound();
            }
            return View(borrowing_record);
        }

        // GET: borrowing_record/Create
        public ActionResult Create()
        {
            ViewBag.ISBN = new SelectList(db.books, "ISBN", "title");
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name");
            return View();
        }

        // POST: borrowing_record/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "borrowing_id,user_id,ISBN,borrow_timestamp,due_timestamp,return_timestamp,status")] borrowing_record borrowing_record)
        {
            if (ModelState.IsValid)
            {
                db.borrowing_record.Add(borrowing_record);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ISBN = new SelectList(db.books, "ISBN", "title", borrowing_record.ISBN);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", borrowing_record.user_id);
            return View(borrowing_record);
        }

        // GET: borrowing_record/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrowing_record borrowing_record = db.borrowing_record.Find(id);
            if (borrowing_record == null)
            {
                return HttpNotFound();
            }
            ViewBag.ISBN = new SelectList(db.books, "ISBN", "title", borrowing_record.ISBN);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", borrowing_record.user_id);
            return View(borrowing_record);
        }

        // POST: borrowing_record/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "borrowing_id,user_id,ISBN,borrow_timestamp,due_timestamp,return_timestamp,status")] borrowing_record borrowing_record)
        {
            if (ModelState.IsValid)
            {
                db.Entry(borrowing_record).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ISBN = new SelectList(db.books, "ISBN", "title", borrowing_record.ISBN);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", borrowing_record.user_id);
            return View(borrowing_record);
        }

        // GET: borrowing_record/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrowing_record borrowing_record = db.borrowing_record.Find(id);
            if (borrowing_record == null)
            {
                return HttpNotFound();
            }
            return View(borrowing_record);
        }

        // POST: borrowing_record/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            borrowing_record borrowing_record = db.borrowing_record.Find(id);
            db.borrowing_record.Remove(borrowing_record);
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
