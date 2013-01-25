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
    public class CommentController : Controller
    {

        private FollowPeersDBEntities db = new FollowPeersDBEntities();

        //
        // GET: /Comment/

        public PartialViewResult Index(string ParentType,string ParentId)
        {
            string UserName = Membership.GetUser().UserName;
            ViewBag.parentType = ParentType;
            ViewBag.ParentId = ParentId;
            ViewBag.UserProfile = db.UserProfiles.SingleOrDefault(u => u.UserName == UserName);
            var comments = db.Comments.Where(c => c.ParentType == ParentType).ToList();
            comments = comments.Where(c => c.ParentId == ParentId).ToList();
            comments = comments.Where(c => c.Flagged == false).ToList();
            return PartialView(comments);

        }

        //
        // GET: /Comment/Details/5

        public ViewResult Details(int id)
        {
            Comment comment = db.Comments.Find(id);
            return View(comment);
        }

        //
        // GET: /Comment/Create

        public ActionResult Create()
        {
            return View();
        }



        

        //
        // POST: /Comment/Create

        [HttpPost]
        public ActionResult Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                var returnTrue = new JsonResult();
                
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details", "PublicationModel", new { id = comment.ParentId });
                    
            }

            return View(comment);  
        }
        //
        // GET: /Comment/Create

        public ActionResult CreateAjax()
        {
            return RedirectToAction("Index");
        }

        //
        // GET: /Comment/Flag
        
        public ActionResult Flag(int id, int PublicationId)
        {
            Comment comment = db.Comments.Find(id);
            comment.Flagged = !comment.Flagged;
            db.SaveChanges();

            return RedirectToAction("Details", "PublicationModel", new { id= PublicationId });
        }
        
        //
        // POST: /Comment/Flag
        /*
        [HttpPost]
        public ActionResult Flag(int id)
        {

            Comment comment = db.Comments.Find(id);
            comment.Flagged = !comment.Flagged;
            db.SaveChanges();

            return RedirectToAction("Index", new { ParentType = comment.ParentType, ParentId = comment.ParentId });
        }
        */
        // POST: /Comment/CreateAjax

        [HttpPost]
        public ActionResult CreateAjax(Comment comment)
        {

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index",comment.ParentType, new{ Id=comment.ParentId});
            }
            return RedirectToAction("Index", comment.ParentType, new { Id = comment.ParentId });
        }

        //
        // GET: /Comment/Edit/5
 
        public ActionResult Edit(int id)
        {
            Comment comment = db.Comments.Find(id);
            return View(comment);
        }

        //
        // POST: /Comment/Edit/5

        [HttpPost]
        public ActionResult Edit(Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(comment);
        }

        //
        // GET: /Comment/Delete/5
 
        public ActionResult Delete(int id)
        {
            Comment comment = db.Comments.Find(id);
            return View(comment);
        }

        //
        // POST: /Comment/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
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