﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.EnterpriseServices.Internal;
using System.Security.Claims;
using System.Threading.Tasks;
using Bug_tracker.Models.Classes;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Bug_tracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string DisplayName { get; set; }
        [InverseProperty("Creater")]
        public virtual ICollection<Tickets> CreatedTickets { get; set; }
        [InverseProperty("Assignee")]
        public virtual ICollection<Tickets> AssignedTickets { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public ApplicationUser()
        {
            Projects = new HashSet<Project>();
            CreatedTickets = new HashSet<Tickets>();
            AssignedTickets = new HashSet<Tickets>();
            TicketAttachments = new HashSet<TicketAttachment>();
            TicketComments = new HashSet<TicketComment>();
            TicketHistories = new HashSet<TicketHistory>();
        }

        public virtual ICollection<Project> Projects { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<Bug_tracker.Models.Classes.Project> Projects { get; set; }
        public System.Data.Entity.DbSet<Bug_tracker.Models.Classes.Tickets> Tickets { get; set; }
        public System.Data.Entity.DbSet<Bug_tracker.Models.Classes.TicketStatus> TicketStatuses { get; set; }
        public System.Data.Entity.DbSet<Bug_tracker.Models.Classes.TicketPriority> TicketPriorities { get; set; }
        public System.Data.Entity.DbSet<Bug_tracker.Models.Classes.TicketType> TicketTypes { get; set; }
        public System.Data.Entity.DbSet<Bug_tracker.Models.TicketAttachment> TicketAttachments { get; set; }
        public System.Data.Entity.DbSet<Bug_tracker.Models.TicketComment> TicketComments { get; set; }
        public System.Data.Entity.DbSet<Bug_tracker.Models.TicketHistory> TicketHistories { get; set; }
    }
}