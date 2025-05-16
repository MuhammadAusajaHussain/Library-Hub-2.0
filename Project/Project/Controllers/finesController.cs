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
    public class finesController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: fines
        public ActionResult Index()
        {
            var fines = db.fines.Include(f => f.borrowing_record);
            return View(fines.ToList());
        }

        // GET: fines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            fine fine = db.fines.Find(id);
            if (fine == null)
            {
                return HttpNotFound();
            }
            return View(fine);
        }

        // GET: fines/Create
        public ActionResult Create()
        {
            ViewBag.borrowing_id = new SelectList(db.borrowing_record, "borrowing_id", "ISBN");
            return View();
        }

        // POST: fines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "borrowing_id,fine_amount,fine_due_timestamp,fine_status")] fine fine)
        {
            if (ModelState.IsValid)
            {
                int lastId = db.fines.Any()
                  ? db.fines.Max(r => r.fine_id)
                  : 0;

                fine.fine_id= lastId + 1;
                db.fines.Add(fine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.borrowing_id = new SelectList(db.borrowing_record, "borrowing_id", "ISBN", fine.borrowing_id);
            return View(fine);
        }

        // GET: fines/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            fine fine = db.fines.Find(id);
            if (fine == null)
            {
                return HttpNotFound();
            }
            ViewBag.borrowing_id = new SelectList(db.borrowing_record, "borrowing_id", "ISBN", fine.borrowing_id);
            return View(fine);
        }

        // POST: fines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "fine_id,borrowing_id,fine_amount,fine_due_timestamp,fine_status")] fine fine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.borrowing_id = new SelectList(db.borrowing_record, "borrowing_id", "ISBN", fine.borrowing_id);
            return View(fine);
        }

        // GET: fines/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            fine fine = db.fines.Find(id);
            if (fine == null)
            {
                return HttpNotFound();
            }
            return View(fine);
        }

        // POST: fines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            fine fine = db.fines.Find(id);
            db.fines.Remove(fine);
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
