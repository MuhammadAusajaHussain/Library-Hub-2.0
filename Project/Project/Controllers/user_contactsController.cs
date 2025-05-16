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
    public class user_contactsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: user_contacts
        public ActionResult Index()
        {
            var user_contacts = db.user_contacts.Include(u => u.user);
            return View(user_contacts.ToList());
        }

        // GET: user_contacts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_contacts user_contacts = db.user_contacts.Find(id);
            if (user_contacts == null)
            {
                return HttpNotFound();
            }
            return View(user_contacts);
        }

        // GET: user_contacts/Create
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name");
            return View();
        }

        // POST: user_contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "user_id,contact")] user_contacts user_contacts)
        {
            if (ModelState.IsValid)
            {
                int lastId = db.user_contacts.Any()
                  ? db.user_contacts.Max(r => r.contact_id)
                  : 0;

                user_contacts.contact_id = lastId + 1;
                db.user_contacts.Add(user_contacts);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", user_contacts.user_id);
            return View(user_contacts);
        }

        // GET: user_contacts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_contacts user_contacts = db.user_contacts.Find(id);
            if (user_contacts == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", user_contacts.user_id);
            return View(user_contacts);
        }

        // POST: user_contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "contact_id,user_id,contact")] user_contacts user_contacts)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user_contacts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", user_contacts.user_id);
            return View(user_contacts);
        }

        // GET: user_contacts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_contacts user_contacts = db.user_contacts.Find(id);
            if (user_contacts == null)
            {
                return HttpNotFound();
            }
            return View(user_contacts);
        }

        // POST: user_contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            user_contacts user_contacts = db.user_contacts.Find(id);
            db.user_contacts.Remove(user_contacts);
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
