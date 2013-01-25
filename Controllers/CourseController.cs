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
    public class CourseController : Controller
    {
        FollowPeersDBEntities db = new FollowPeersDBEntities();
        string name = Membership.GetUser().UserName;
        static UserProfile myprofile;
        //
        // GET: /Course/

        public ViewResult Index()
        {
            myprofile = db.UserProfiles.SingleOrDefault(p => p.UserName == name);
            return View(db.Courses.ToList());
        }

        //
        // GET: /Course/Mine

        public ViewResult Mine()
        {
            myprofile = db.UserProfiles.SingleOrDefault(p => p.UserName == name);
            return View(myprofile.Courses.ToList());
        }

        //
        // GET: /Course/Search

        public ViewResult Search(string q)
        {
            myprofile = db.UserProfiles.SingleOrDefault(p => p.UserName == name);
            if (q == null || q == "")
            {
                ViewBag.SearchString = "Showing all courses";
                ViewBag.CourseCount = db.Courses.ToList().Count;
                return View(db.Courses.ToList());
            }
            else
            {
                ViewBag.SearchString = "Showing results for: " + q;

                IEnumerable<Course> result = from Course in db.Courses
                                             where (Course.CourseName.Contains(q))
                                             orderby Course.CourseName
                                             select Course;

                ViewBag.CourseCount = result.ToList().Count;
                return View(result.ToList());
            }
        }


        //
        // GET: /Course/Details/5

        public ViewResult Details(int id)
        {
            Course course = db.Courses.Find(id);
            return View(course);
        }

        //
        // GET: /Course/Create

        public ActionResult Create()
        {
            string name = Membership.GetUser().UserName;
            UserProfile user = db.UserProfiles.SingleOrDefault(p => p.UserName == name);

            int userID;
            userID = user.UserProfileId;
            ViewBag.UserProfile = user;
            ViewBag.userID = userID;
            return View();

        } 

        //
        // POST: /Course/Create

        [HttpPost]
        public ActionResult Create(Course course)
        {
            string name = Membership.GetUser().UserName;
            UserProfile user = db.UserProfiles.SingleOrDefault(p => p.UserName == name);
            if (ModelState.IsValid)
            {
                user.Courses.Add(course);
                db.Entry(user).State = EntityState.Modified;
                
                //db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Mine");
            }

            return View(course);
        }
        
        //
        // GET: /Course/Edit/5
 
        public ActionResult Edit(int id)
        {
            Course course = db.Courses.Find(id);
            return View(course);
        }

        //
        // POST: /Course/Edit/5

        [HttpPost]
        public ActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        //
        // GET: /Course/Delete/5
 
        public ActionResult Delete(int id)
        {
            Course course = db.Courses.Find(id);
            return View(course);
        }

        //
        // POST: /Course/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Course/Delete/5


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}