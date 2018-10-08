using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bug_tracker;
using Bug_tracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Bug_tracker.Controllers
{
   
    public class ApplicationUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ApplicationUsers
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }
        [Authorize(Roles ="Admin")]
        public ActionResult ChangeRole(string id)
        {
            var model = new UserRoleViewModel();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            var user = userManager.FindById(id);

            model.Id = id;
            model.Name = User.Identity.Name;

            var roles = roleManager.Roles.ToList();
            var userRoles = userManager.GetRoles(id);

            model.Roles = new MultiSelectList(roles, "Name", "Name", userRoles);

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeRole(UserRoleViewModel model)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            //STEP 1: Find the user
            var user = userManager.FindById(model.Id);

            //STEP 2: Get UserRoles:
            var userRoles = userManager.GetRoles(user.Id);

            //STEP 3: Remove the roles from the user
            foreach (var role in userRoles)
            {
                userManager.RemoveFromRole(user.Id, role);
            }

            //STEP 4: Add roles to the user
            foreach (var role in model.SelectedRoles)
            {
                userManager.AddToRole(user.Id, role);
            };

            //STEP 5: Refresh authentication cookies so the roles are updated instantly
            //var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            //signInManager.SignIn(user, isPersistent: false, rememberBrowser: false);

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