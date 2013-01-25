using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FollowPeers.Models;

namespace FollowPeers.Controllers
{ 
    public class SignUpController : Controller
    {
        private FollowPeersDBEntities db = new FollowPeersDBEntities();

        //
        // GET: /SignUp/

        public ViewResult Index()
        {
            return View(db.UserProfiles.ToList());
        }

        //
        // GET: /SignUp/Details/5

        public ViewResult Details(int id)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            return View(userprofile);
        }

        //
        // GET: /SignUp/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /SignUp/Create

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
        // GET: /SignUp/Edit/5
 
        public ActionResult Edit(int id)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            return View(userprofile);
        }

        //
        // POST: /SignUp/Edit/5

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
        // GET: /SignUp/Delete/5
 
        public ActionResult Delete(int id)
        {
            UserProfile userprofile = db.UserProfiles.Find(id);
            return View(userprofile);
        }

        //
        // POST: /SignUp/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            UserProfile userprofile = db.UserProfiles.Find(id);
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