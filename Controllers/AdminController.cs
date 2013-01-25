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
    public class AdminController : Controller
    {
        private FollowPeersDBEntities db = new FollowPeersDBEntities();
        //
        // GET: /Admin/

        public ViewResult Index()
        {
            string name = Membership.GetUser().UserName;
            UserProfile me = db.UserProfiles.SingleOrDefault(p => p.UserName == name);
            var list = db.UserProfiles.ToList();
            list.Remove(me);
            return View(list);
        }

        //
        // GET: /Admin/Details/5

        public ViewResult Details(int id)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            return View(userprofile);
        }

        //
        // GET: /Admin/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Admin/Create

        [HttpPost]
        public ActionResult Create(UserProfile userprofile)
        {
            if (ModelState.IsValid)
            {
                db.UserProfiles.Add(userprofile);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(userprofile);
        }
        
        //
        // GET: /Admin/Edit/5
 
        public ActionResult Edit(int id)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            return View(userprofile);
        }

        //
        // POST: /Admin/Edit/5

        [HttpPost]
        public ActionResult Edit(UserProfile userprofile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userprofile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userprofile);
        }

        //
        // GET: /Admin/Delete/5
 
        public ActionResult Delete(int id)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            return View(userprofile);
        }

        public ActionResult CleanDatabase()
        {
            Database.SetInitializer<FollowPeersDBEntities>(null);
            return RedirectToAction("Index");
        }

        //
        // POST: /Admin/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            UserProfile userprofile = db.UserProfiles.Find(id);
            IQueryable<Notification> allnotis = from noti in db.Notifications where noti.UserProfile.UserProfileId== id select noti;
            foreach (var noti in allnotis)
            {
                db.Notifications.Remove(noti);
            }
            db.SaveChanges();

            IQueryable<PublicationModel> allpublis = from publi in db.PublicationModels where publi.UserProfile.UserProfileId == id select publi;
            foreach (var publi in allpublis)
            {
                db.PublicationModels.Remove(publi);
            }
            db.SaveChanges();

            IQueryable<PatentModel> allpatents = from patent in db.PatentModels where patent.UserProfile.UserProfileId == id select patent;
            foreach (var patent in allpatents)
            {
                db.PatentModels.Remove(patent);
            }
            db.SaveChanges();

            IQueryable<Tier> alltiers = from tier in db.Tiers where tier.UserProfile.UserProfileId == id select tier;
            foreach (var tier in alltiers)
            {
                db.Tiers.Remove(tier);
            }
            db.SaveChanges();

            IQueryable<Relationship> allrelationships = from relationship in db.Relationships where relationship.personAName == userprofile.UserName orderby relationship.personBName==userprofile.UserName select relationship;
            foreach (var relationship in allrelationships)
            {
                db.Relationships.Remove(relationship);
            }
            db.SaveChanges();

            IQueryable<NoticeComment> allnoticecomments = from noticecomment in db.NoticeComments where noticecomment.commenter.UserProfileId==userprofile.UserProfileId || noticecomment.commenterId==id select noticecomment;
            foreach (var noticecomment in allnoticecomments)
            {
                db.NoticeComments.Remove(noticecomment);
            }
            db.SaveChanges();

            IQueryable<Update> allupdates = from update in db.Updates select update;
            foreach (var update in allupdates)
            {
                if ((update.owner == id) || (update.UserProfiles.Contains(userprofile)))
                {
                    IQueryable<NoticeComment> allnoticecomments2 = from noticecomment in db.NoticeComments select noticecomment;
                    foreach (var noticecomment in allnoticecomments2)
                    {
                        if (noticecomment.update.UpdateId==update.UpdateId)
                            db.NoticeComments.Remove(noticecomment);
                        db.SaveChanges();
                    }
                    db.Updates.Remove(update);
                }
            }
            db.SaveChanges();


            IQueryable<Comment> allcomments = from comment in db.Comments where comment.SubmittedBy == userprofile.UserName select comment;
            foreach (var comment in allcomments)
            {
                db.Comments.Remove(comment);
            }
            db.SaveChanges();

            IQueryable<Group> allgroups = from groups in db.Groups select groups;
            foreach (var groups in allgroups)
            {
                if (groups.OwnerId == id || groups.Members.Contains(userprofile))
                    db.Groups.Remove(groups);
            }
            db.SaveChanges();

            IQueryable<Job> alljobs = from job in db.Jobs select job;
            foreach (var job in alljobs)
            {
                if (job.ownerId == id)
                    db.Jobs.Remove(job);
            }
            db.SaveChanges();



            db.UserProfiles.Remove(userprofile);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}