using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Project.Models;

namespace Project.Controllers
{
    public class CalFineController : Controller
    {
        private readonly LMSEntities db = new LMSEntities();

        // GET: CalFine
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "Login");

            int userId = (int)Session["UserId"];
            DateTime today = DateTime.Now.Date;

            // Load borrow records (borrowed/late or returned-with-unpaid)
            var records = db.borrowing_record
                .Include(b => b.book)
                .Where(b =>
                    b.user_id == userId &&
                    (
                        b.status == "borrowed" ||
                        b.status == "late" ||
                        (b.status == "returned" &&
                         db.fines.Any(f => f.borrowing_id == b.borrowing_id && f.fine_status == "unpaid"))
                    )
                )
                .ToList();

            var vmList = new List<CalFineViewModel>();

            foreach (var r in records)
            {
                // Compute overdue days
                int overdueDays = r.due_timestamp.HasValue
                    ? Math.Max(0, (today - r.due_timestamp.Value.Date).Days)
                    : 0;

                // Lookup or upsert fine
                var existingFine = db.fines.FirstOrDefault(f => f.borrowing_id == r.borrowing_id);
                decimal fineAmount = 0;
                string fineStatus = "unpaid";
                int? fineId = null;

                if (overdueDays > 0)
                {
                    fineAmount = overdueDays;

                    if (existingFine != null)
                    {
                        if (existingFine.fine_status == "unpaid")
                        {
                            existingFine.fine_amount = fineAmount;
                            existingFine.fine_due_timestamp = DateTime.Now;
                            db.Entry(existingFine).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        fineStatus = existingFine.fine_status;
                        fineId = existingFine.fine_id;
                    }
                    else
                    {
                        // new fine_id
                        int nextId = db.fines.Any() ? db.fines.Max(f => f.fine_id) + 1 : 1;
                        existingFine = new fine
                        {
                            fine_id = nextId,
                            borrowing_id = r.borrowing_id,
                            fine_amount = fineAmount,
                            fine_due_timestamp = DateTime.Now,
                            fine_status = "unpaid"
                        };
                        db.fines.Add(existingFine);
                        db.SaveChanges();    // <— Persist immediately
                        fineStatus = "unpaid";
                        fineId = nextId;
                    }
                }
                else if (existingFine != null && existingFine.fine_status == "unpaid")
                {
                    fineAmount = existingFine.fine_amount ?? 0;
                    fineStatus = existingFine.fine_status;
                    fineId = existingFine.fine_id;
                }

                // Only list nonzero unpaid fines
                if (fineAmount > 0 && fineId.HasValue)
                {
                    vmList.Add(new CalFineViewModel
                    {
                        FineId = fineId.Value,
                        BorrowingId = r.borrowing_id,
                        BookTitle = r.book?.title ?? "Unknown",
                        DueDate = r.due_timestamp,
                        DaysOverdue = overdueDays,
                        FineAmount = fineAmount,
                        FineStatus = fineStatus
                    });
                }
            }

            return View(vmList);
        }

        // POST: CalFine/PayFine
        [HttpPost]
        public ActionResult PayFine(int fineId)
        {
            var fine = db.fines.Find(fineId);
            if (fine != null && fine.fine_status == "unpaid")
            {
                fine.fine_status = "paid";
                fine.fine_due_timestamp = DateTime.Now;
                db.Entry(fine).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Fine paid successfully.";
            }
            return RedirectToAction("Index");
        }
    }

    public class CalFineViewModel
    {
        public int FineId { get; set; }
        public int BorrowingId { get; set; }
        public string BookTitle { get; set; }
        public DateTime? DueDate { get; set; }
        public int DaysOverdue { get; set; }
        public decimal FineAmount { get; set; }
        public string FineStatus { get; set; }
    }
}
