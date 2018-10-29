using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bug_tracker.Models;
using Bug_tracker.Models.Classes;
using Microsoft.AspNet.Identity;
using Bug_tracker.helper;
using System.Net.Mail;
using System.Web.Configuration;

namespace Bug_tracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db { get; set; }
        private UserRoleHelper UserRoleHelper { get; set; }
        public TicketsController()
        {
            db = new ApplicationDbContext();
            UserRoleHelper = new UserRoleHelper();
        }

        // GET: Tickets
        public ActionResult Index(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                return View(db.Tickets.Include(t => t.TicketPriority).Include(t => t.Project).Include(t => t.TicketStatus).Include(t => t.TicketType).Where(p => p.CreaterId == User.Identity.GetUserId()).ToList());
            }
            return View(db.Tickets.Include(t => t.TicketPriority).Include(t => t.Project).Include(t => t.TicketStatus).Include(t => t.TicketType).ToList());
        }

        //Get UsreTickets
        public ActionResult yourtickets()
        {
            string userID = User.Identity.GetUserId();
            if (User.IsInRole("Submitter"))
            {
                var tickets = db.Tickets.Where(t => t.CreaterId == userID).Include(t => t.Creater).Include(t => t.Assignee).Include(t => t.Project);
                return View("Index", tickets.ToList());
            }
            if (User.IsInRole("Developer"))
            {
                var tickets = db.Tickets.Where(t => t.AssigneeId == userID).Include(t => t.Comments).Include(t => t.Creater).Include(t => t.Assignee).Include(t => t.Project);
                return View("Index", tickets.ToList());
            }
            if (User.IsInRole("Project Manager"))
            {
                return View(db.Tickets.Include(t => t.TicketPriority).Include(t => t.Project).Include(t => t.TicketStatus).Include(t => t.TicketType).Where(p => p.AssigneeId == userID).ToList());
            }

            return View("Index");
        }

        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult AssignDeveloper(int id)
        { 
            var model = new AssignDevelopersTicketModel();
            var ticket = db.Tickets.FirstOrDefault(p => p.Id == id);
            var userRoleHelper = new UserRoleHelper();
            var users = userRoleHelper.UsersInRole("Developer");
            model.TicketId = id;
            model.DeveloperList = new SelectList(users, "Id", "DisplayName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult AssignDeveloper(AssignDevelopersTicketModel model)
        {
            var ticket = db.Tickets.FirstOrDefault(p => p.Id == model.TicketId);
            ticket.AssigneeId = model.SelectedDeveloperId;     
            var user = db.Users.FirstOrDefault(p => p.Id == model.SelectedDeveloperId);
            var personalEmailService = new PersonalEmailServices();
            var mailMessage = new MailMessage(WebConfigurationManager.AppSettings["emailto"], user.Email);
            mailMessage.Body = "Someone Assign you a ticket";
            mailMessage.Subject = "Assign";
            mailMessage.IsBodyHtml = true;
            personalEmailService.Send(mailMessage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }
        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager,Submitter,Developer")]
        public ActionResult CreateComment(int id, string body)
        {
            var ticket = db.Tickets.Where(p => p.Id == id).FirstOrDefault();
            var userId = User.Identity.GetUserId();
            if (ticket == null)
            {
                return HttpNotFound();
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                TempData["Errormessage"] = "Comment is Mandatory";
                return RedirectToAction("Details", new { id });
            }          
                var comment = new TicketComment();
                comment.UserId = User.Identity.GetUserId();
                comment.TicketId = ticket.Id;
                comment.Created = DateTime.Now;
                comment.Comment = body;
                db.TicketComments.Add(comment);
                var user = db.Users.FirstOrDefault(p => p.Id == comment.UserId);
                var personalEmailService = new PersonalEmailServices();
                var mailMessage = new MailMessage(WebConfigurationManager.AppSettings["emailto"], user.Email);
                mailMessage.Body = "Someone commented on your Ticket";
                mailMessage.Subject = "Comment";
                mailMessage.IsBodyHtml = true;
                personalEmailService.Send(mailMessage);
            
            db.SaveChanges();
            return RedirectToAction("Details", new { id });
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            ViewBag.AssigneeId = new SelectList(db.Users, "Id", "DisplayName");
            ViewBag.CreaterId = new SelectList(db.Users, "Id", "DisplayName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
        public ActionResult Create([Bind(Include = "Id,Name,Description,TicketTypeId,TicketPriorityId,ProjectId")] Tickets tickets)
        {
            if (ModelState.IsValid)
            {
                if (tickets == null)
                {
                    return HttpNotFound();
                }
                tickets.TicketStatusId = 3;
                db.Tickets.Add(tickets);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", tickets.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
            return View(tickets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Submitter")]
        public ActionResult CreateAttachment(int ticketId, [Bind(Include = "Id,Description,TicketTypeId")] TicketAttachment ticketAttachment, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var tickets = db.Tickets.FirstOrDefault(t => t.Id == ticketId);
                if (!ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    TempData["ErrorMessage"] = "uploading image is Mandatory";
                    return RedirectToAction("Details", new { id = ticketId });
                }
                if (image == null)
                {
                    return HttpNotFound();
                }
                var FileUsed = Path.GetFileName(image.FileName);
                if (!Directory.Exists(Server.MapPath("~/Uploads/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Uploads/"));
                }
                image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), FileUsed));
                ticketAttachment.FilePath = "/Uploads/" + FileUsed;
                ticketAttachment.UserId = User.Identity.GetUserId();
                ticketAttachment.Created = DateTime.Now;
                ticketAttachment.UserId = User.Identity.GetUserId();
                ticketAttachment.TicketId = ticketId;
                db.TicketAttachments.Add(ticketAttachment);
                var user = db.Users.FirstOrDefault(p => p.Id == ticketAttachment.UserId);
                var personalEmailService = new PersonalEmailServices();
                var mailMessage = new MailMessage(
                WebConfigurationManager.AppSettings["emailto"], user.Email);
                mailMessage.Body = "You have a new attachment";
                mailMessage.Subject = "New Attachment";
                mailMessage.IsBodyHtml = true;
                personalEmailService.Send(mailMessage);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = ticketId });
            }
            return View(ticketAttachment);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssigneeId = new SelectList(db.Users, "Id", "DisplayName", tickets.AssigneeId);
            ViewBag.CreaterId = new SelectList(db.Users, "Id", "DisplayName", tickets.CreaterId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", tickets.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", tickets.TicketTypeId);
            return View(tickets);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager,Developer")]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,TicketTypeId,TicketPriorityId,CreaterId,TicketStatusId,AssigneeId,ProjectId")] Tickets tickets)
        {if (ModelState.IsValid)
            {
                var dateChanged = DateTimeOffset.Now;
                var changes = new List<TicketHistory>();

                var dbTicket = db.Tickets.First(p => p.Id == tickets.Id);

                dbTicket.Name = tickets.Name;
                dbTicket.Description = tickets.Description;
                dbTicket.TicketTypeId = tickets.TicketTypeId;
                dbTicket.Updated = dateChanged;
                dbTicket.TicketPriorityId = tickets.TicketPriorityId;
                dbTicket.TicketStatusId = tickets.TicketStatusId;
                dbTicket.TicketTypeId = tickets.TicketTypeId;

                var originalValues = db.Entry(dbTicket).OriginalValues;
                var currentValues = db.Entry(dbTicket).CurrentValues;

                foreach(var property in originalValues.PropertyNames)
                {
                    var originalValue = originalValues[property]?.ToString();
                    var currentValue = currentValues[property]?.ToString();

                    if (originalValue != currentValue)
                    {
                        var history = new TicketHistory();
                        history.Changed = dateChanged;
                        history.NewValue = GetValueFromKey(property, currentValue);
                        history.OldValue = GetValueFromKey(property, originalValue);
                        history.Property = property;
                        history.TicketId = dbTicket.Id;
                        history.UserId = User.Identity.GetUserId();
                        changes.Add(history);
                    }
                }

                db.TicketHistories.AddRange(changes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tickets);
        }

        private string GetValueFromKey(string propertyName, string key)
        {
            if (propertyName == "TicketTypeId")
            {
                return db.TicketTypes.Find(Convert.ToInt32(key)).Name;
            }
            if (propertyName == "TicketPriorityId")
            {
                return db.TicketTypes.Find(Convert.ToInt32(key)).Name;
            }
            if (propertyName == "TicketStatusId")
            {
                return db.TicketTypes.Find(Convert.ToInt32(key)).Name;
            }

            return key;
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tickets tickets = db.Tickets.Find(id);
            db.Tickets.Remove(tickets);
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
