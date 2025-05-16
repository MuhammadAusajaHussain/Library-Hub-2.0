using System;
using System.Linq;
using System.Web.Mvc;
using Project.Models;

namespace Project.Controllers
{
    public class LoginController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Index(string email, string password)
        {
            var user = db.users.FirstOrDefault(u => u.email == email && u.password == password);

            if (user != null)
            {
                // Set last login time
                user.last_login_timestamp = DateTime.Now;
                db.SaveChanges();

                // Store session
                Session["UserId"] = user.user_id;
                Session["UserName"] = user.user_name;
                Session["Role"] = user.role != null ? user.role.role_name : "member";

                if (user.role != null && user.role.role_name.ToLower() == "admin")
                {
                    // Redirect to Admin Dashboard
                    return RedirectToAction("Index", "AdminDashboard/Dashboard");
                }
                else
                {
                    // Redirect to User Dashboard
                    return RedirectToAction("Index", "UserDashboard/Dashboard");
                }
               
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }


        // GET: Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public ActionResult Register(user model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (db.users.Any(u => u.email == model.email))
                {
                    ViewBag.Error = "Email already registered.";
                    return View(model);
                }

                model.registration_timestamp = DateTime.Now;
                model.role_id = db.roles.FirstOrDefault(r => r.role_name == "member")?.role_id; // default role

                db.users.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
