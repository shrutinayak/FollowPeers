using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FollowPeers.Models;
using System.Web.Security;

namespace FollowPeers.Controllers
{ 
    public class MessageController : Controller
    {
        private FollowPeersDBEntities db = new FollowPeersDBEntities();

        //
        // GET: /Message/

        public ViewResult Index()
        {
            string UserName = Membership.GetUser().UserName;
            var RecievedMessages = db.Messages.Where(m => m.Reciever == UserName).ToList();
            var UnTrashedMessages = RecievedMessages.Where(m => m.Trashed == false).ToList();
            ViewBag.UserName = UserName;
            return View(UnTrashedMessages);
        }

        //
        // GET: /Message/Sent

        public ViewResult Sent()
        {
            string UserName = Membership.GetUser().UserName;
            var SentMessages = db.Messages.Where(m => m.Sender == UserName).ToList();
            ViewBag.UserName = UserName;
            return View(SentMessages);
        }

        //
        // GET: /Message/Starred
        public ViewResult Starred() {
            string UserName = Membership.GetUser().UserName;
            var SentMessages = db.Messages.Where(m => m.Sender == UserName).ToList();
            var RecievedMessages = db.Messages.Where(m => m.Reciever == UserName).ToList();
            SentMessages.AddRange(RecievedMessages);
            var StarredMessages = SentMessages.Where(m => m.Starred).ToList();
            return View(StarredMessages);
        }
        // GET: /Message/Trashed
        public ViewResult Trashed()
        {
            string UserName = Membership.GetUser().UserName;
            var SentMessages = db.Messages.Where(m => m.Sender == UserName).ToList();
            var RecievedMessages = db.Messages.Where(m => m.Reciever == UserName).ToList();
            SentMessages.AddRange(RecievedMessages);
            var TrashedMessages = SentMessages.Where(m => m.Trashed).ToList();
            return View(TrashedMessages);
        }
        // GET: /Message/Details/5

        public ViewResult Details(int id)
        {
            Message message = db.Messages.Find(id);
            return View(message);
        }

        //
        // GET: /Message/Create

        public ActionResult Create(int Receiver = 0)
        {
            var message = new Message();
            if (Receiver != 0)
            {
                UserProfile rec = db.UserProfiles.SingleOrDefault(u => u.UserProfileId == Receiver);
                message.Reciever = rec.UserName;
                return View(message);
            }
            return View();
        } 

        //
        // POST: /Message/Create

        [HttpPost]
        public ActionResult Create(Message message)
        {
            if (ModelState.IsValid)
            {
                message.Sender = Membership.GetUser().UserName;
                message.TimeSent = DateTime.Now.ToString();
                message.Starred = false;
                message.Trashed = false;
                db.Messages.Add(message);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(message);
        }
        //
        // GET: /Message/Reply
        public ActionResult Reply()
        {
            return View();
        }

        //
        // POST: /Message/Create

        [HttpPost]
        public ActionResult Reply(Message message)
        {
            if (ModelState.IsValid)
            {
                message.Sender = Membership.GetUser().UserName;
                message.TimeSent = DateTime.Now.ToString();
                message.Starred = false;
                message.Trashed = false;
                db.Messages.Add(message);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
        //
        // GET: /Message/Edit/5
 
        public ActionResult Edit(int id)
        {
            Message message = db.Messages.Find(id);
            return View(message);
        }

        //
        // POST: /Message/Edit/5

        [HttpPost]
        public ActionResult Edit(Message message)
        {
            if (ModelState.IsValid)
            {
                db.Entry(message).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(message);
        }

        //
        // GET: /Message/Delete/5
 
        public ActionResult Delete(int id)
        {
            Message message = db.Messages.Find(id);
            return View(message);
        }

        //
        // POST: /Message/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: /Message/Star/5
        public ActionResult Star(int id)
        {
            Message message = db.Messages.Find(id);
            bool temp = message.Starred;
            message.Starred = !temp;
            db.Messages.Find(id).Starred = !temp;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /Message/Trash/5
        public ActionResult Trash(int id)
        {
            Message message = db.Messages.Find(id);
            bool temp = message.Trashed;
            message.Trashed = !temp;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /Message/UserNames/5
        /*public JsonResult FindUserNames(string searchText= "", int maxResults = 0)
        {
            return
        }*/

    }
}