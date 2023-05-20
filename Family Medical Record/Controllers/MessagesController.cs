using Family_Medical_Record.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Net;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Ajax.Utilities;
using System;

namespace Family_Medical_Record.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        FMRDB db = new FMRDB();
        ApplicationDbContext ADC = new ApplicationDbContext();

        // GET: Messages
        public ActionResult Index()
        {
            string uname = User.Identity.GetUserName();

            return View(db.Messages.OrderByDescending(m => m.MessageId).DistinctBy(m => m.Sender != uname && m.Receptient != uname).Where(m => m.Sender == uname || m.Receptient == uname).OrderBy(m => m.TimeStamp).ToList());
        }

        [HttpPost]
        public ActionResult Send([Bind(Include = "Text,Receptient")] Message message)
        {
            message.Sender = User.Identity.GetUserName();
            message.TimeStamp = DateTime.Now;
            message.HasSeen = false;

            if (ModelState.IsValid)
            {
                db.Messages.Add(message);
                db.SaveChanges();
                return RedirectToAction("Chat", new {id = message.Receptient});
            }

            return View(message);
        }

        public ActionResult Chat(string id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ADC));
            string rec = id;
            var currentUser = UserManager.Users.Where(x => x.UserName == rec).First();

            if(currentUser == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string sen = User.Identity.GetUserName();

            ViewBag.Rec = id;

            var messages = db.Messages.Where(m => m.Receptient == sen && m.HasSeen == false).ToList();

            foreach(Message m in messages)
            {
                m.HasSeen = true;
                db.Entry(m).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();

            return View(db.Messages.Where(m => (m.Receptient == sen && m.Sender == rec) || (m.Sender == sen && m.Receptient == rec)).OrderBy(m => m.TimeStamp).ToList());
        }

        [ChildActionOnly]
        public ActionResult _MessagesPartial()
        {
            string uname = User.Identity.GetUserName();

            var messages = db.Messages.Where(m => m.Receptient == uname && m.HasSeen == false).ToList();

            return PartialView(messages);
        }
    }
}