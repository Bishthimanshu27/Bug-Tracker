namespace Bug_tracker.Migrations
{
    using Bug_tracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Bug_tracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "BugTracker.Models.ApplicationDbContext";
        }

        protected override void Seed(Bug_tracker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }
            ApplicationUser adminUser;
            if (!context.Users.Any(item => item.Email == "admin@admin.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@admin.com";
                adminUser.Email = "admin@admin.com";
                adminUser.LastName = "Admin";
                adminUser.FirstName = "Himanshu";
                adminUser.DisplayName = "Himanshu Bisht";
                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context.Users.FirstOrDefault(item => item.UserName == "admin@admin.com");
            }
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            ApplicationUser DemoAdmin = new ApplicationUser();
            ApplicationUser DemoProjectManger = new ApplicationUser();
            ApplicationUser DemoDeveloper = new ApplicationUser();
            ApplicationUser DemoSubmitter = new ApplicationUser();

            if (!context.Users.Any(item => item.Email == "DemoAdmin@admin.com"))
            {
                DemoAdmin = new ApplicationUser();
                DemoAdmin.UserName = "DemoAdmin@admin.com";
                DemoAdmin.Email = "DemoAdmin@admin.com";
                DemoAdmin.LastName = "Admin";
                DemoAdmin.FirstName = "Himanshu";
                DemoAdmin.DisplayName = "Himanshu Bisht";
                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context.Users.FirstOrDefault(item => item.UserName == "DemoAdmin@admin.com");
            }
            if (!userManager.IsInRole(DemoAdmin.Id, "DemoAdmin"))
            {
                userManager.AddToRole(DemoAdmin.Id, "DemoAdmin");
            }

            if (!context.Users.Any(item => item.Email == "DemoProjectManger@admin.com"))
            {
                DemoProjectManger = new ApplicationUser();
                DemoProjectManger.UserName = "DemoProjectManger@admin.com";
                DemoProjectManger.Email = "DemoProjectManger@admin.com";
                DemoProjectManger.LastName = "Admin";
                DemoProjectManger.FirstName = "Himanshu";
                DemoProjectManger.DisplayName = "Himanshu Bisht";
                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context.Users.FirstOrDefault(item => item.UserName == "DemoProjectManger@admin.com");
            }
            if (!userManager.IsInRole(DemoProjectManger.Id, "DemoProjectManger"))
            {
                userManager.AddToRole(DemoProjectManger.Id, "DemoProjectManger");
            }

            if (!context.Users.Any(item => item.Email == "DemoDeveloper@admin.com"))
            {
                DemoDeveloper = new ApplicationUser();
                DemoDeveloper.UserName = "DemoDeveloper@admin.com";
                DemoDeveloper.Email = "DemoDeveloper@admin.com";
                DemoDeveloper.LastName = "Admin";
                DemoDeveloper.FirstName = "Himanshu";
                DemoDeveloper.DisplayName = "Himanshu Bisht";
                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                DemoDeveloper = context.Users.FirstOrDefault(item => item.UserName == "DemoDeveloper@admin.com");
            }
            if (!userManager.IsInRole(DemoDeveloper.Id, "DemoDeveloper"))
            {
                userManager.AddToRole(DemoDeveloper.Id, "DemoDeveloper");
            }

            if (!context.Users.Any(item => item.Email == "DemoSubmitter@admin.com"))
            {
                DemoSubmitter = new ApplicationUser();
                DemoSubmitter.UserName = "DemoSubmitter@admin.com";
                DemoSubmitter.Email = "DemoSubmitter@admin.com";
                DemoSubmitter.LastName = "Admin";
                DemoSubmitter.FirstName = "Himanshu";
                DemoSubmitter.DisplayName = "Himanshu Bisht";
                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context.Users.FirstOrDefault(item => item.UserName == "DemoSubmitter@admin.com");
            }
            if (!userManager.IsInRole(DemoSubmitter.Id, "DemoSubmitter"))
            {
                userManager.AddToRole(DemoSubmitter.Id, "DemoSubmitter");
            }

            context.TicketTypes.AddOrUpdate(x => x.Id,
                new Models.Classes.TicketType() { Id = 1, Name = "Bug Fixes" },
                new Models.Classes.TicketType() { Id = 2, Name = "Software Update" },
                new Models.Classes.TicketType() { Id = 3, Name = "Adding Helpers" },
                new Models.Classes.TicketType() { Id = 4, Name = "Database errors" });

            context.TicketPriorities.AddOrUpdate(x => x.Id,
                new Models.Classes.TicketPriority() { Id = 1, Name = "High" },
                new Models.Classes.TicketPriority() { Id = 2, Name = "Medium" },
                new Models.Classes.TicketPriority() { Id = 3, Name = "Low" },
                new Models.Classes.TicketPriority() { Id = 4, Name = "Urgent" });

            context.TicketStatuses.AddOrUpdate(x => x.Id,
                new Models.Classes.TicketStatus() { Id = 1, Name = "Finished" },
                new Models.Classes.TicketStatus() { Id = 2, Name = "Started" },
                new Models.Classes.TicketStatus() { Id = 3, Name = "Not Started" },
                new Models.Classes.TicketStatus() { Id = 4, Name = "In progress" });
            context.SaveChanges();
        }
    }
}