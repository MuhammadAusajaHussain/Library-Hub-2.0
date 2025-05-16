using System;
using System.Linq;
using System.Web.Mvc;
using Project.Models;

namespace Project.Controllers
{
    public class UserProfileController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: UserProfile
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "Login");

            int userId = (int)Session["UserId"];
            var user = db.users.Include("role").Include("user_contacts").FirstOrDefault(u => u.user_id == userId);

            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // POST: UserProfile/Update
        [HttpPost]
        public ActionResult Update(user updatedUser)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "Login");

            int userId = (int)Session["UserId"];
            var user = db.users.Include("role").Include("user_contacts").FirstOrDefault(u => u.user_id == userId);

            if (user == null)
                return HttpNotFound();

            // Update user table fields
            user.user_name = updatedUser.user_name;
            user.email = updatedUser.email;

            // Handle the contact update or addition
            var existingContact = user.user_contacts.FirstOrDefault();
            if (existingContact != null)
            {
                // Update existing contact
                existingContact.contact = updatedUser.user_contacts.FirstOrDefault()?.contact;
            }
            else
            {
                // No contact exists, create a new one
                var newContact = new user_contacts
                {
                    user_id = userId,
                    contact = updatedUser.user_contacts.FirstOrDefault()?.contact
                };
                db.user_contacts.Add(newContact);
            }

            // Save changes to the database
            db.SaveChanges();

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }
    }
}
