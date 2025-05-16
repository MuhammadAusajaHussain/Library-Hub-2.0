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
    public class login_historyController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: login_history
        public ActionResult Index()
        {
            var login_history = db.login_history.Include(l => l.user);
            return View(login_history.ToList());
        }

        // GET: login_history/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            login_history login_history = db.login_history.Find(id);
            if (login_history == null)
            {
                return HttpNotFound();
            }
            return View(login_history);
        }

        // GET: login_history/Create
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name");
            return View();
        }

        // POST: login_history/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "user_id,login_time,logout_time,ip_address")] login_history login_history)
        {
            if (ModelState.IsValid)
            {

                int lastId = db.login_history.Any()
                  ? db.login_history.Max(r => r.login_id)
                  : 0;

                login_history.login_id = lastId + 1;
                db.login_history.Add(login_history);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", login_history.user_id);
            return View(login_history);
        }

        // GET: login_history/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            login_history login_history = db.login_history.Find(id);
            if (login_history == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", login_history.user_id);
            return View(login_history);
        }

        // POST: login_history/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "login_id,user_id,login_time,logout_time,ip_address")] login_history login_history)
        {
            if (ModelState.IsValid)
            {
                db.Entry(login_history).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", login_history.user_id);
            return View(login_history);
        }

        // GET: login_history/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            login_history login_history = db.login_history.Find(id);
            if (login_history == null)
            {
                return HttpNotFound();
            }
            return View(login_history);
        }

        // POST: login_history/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            login_history login_history = db.login_history.Find(id);
            db.login_history.Remove(login_history);
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
