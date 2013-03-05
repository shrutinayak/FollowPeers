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
    public class JobsController : Controller
    {
        private FollowPeersDBEntities followPeersDB = new FollowPeersDBEntities();

        //
        // GET: /Default1/

        public ViewResult Index()
        {
            string name = Membership.GetUser().UserName;
            int userID;
            UserProfile user = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            userID = user.UserProfileId;
            var result = from n in followPeersDB.Jobs
                         select n;

            return View(result.ToList());
        }

        //
        // GET: /Default1/Details/5

        public ActionResult Create()
        {
            string name = Membership.GetUser().UserName;
            UserProfile user = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);

            int userID;
            userID = user.UserProfileId;

            ViewBag.userID = userID;
            return View();
        }


        //
        // POST: /Default1/Create


        [HttpPost]
        public ActionResult Create(Jobs jobmodel)
        {
            if (ModelState.IsValid)
            {
                string name = Membership.GetUser().UserName;
                UserProfile user = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
                if (jobmodel.Enddate == null)
                {
                    jobmodel.Enddate = DateTime.Now;
                }
                if (jobmodel.publishDate == null)
                {
                    jobmodel.publishDate = DateTime.Now;
                }
                jobmodel.ownerID = user.UserProfileId;


                jobmodel.UserProfile = user;
                int jobid = followPeersDB.Jobs.Count() + 1;
                user.Jobs.Add(jobmodel);
                CreateUpdates("Published a new job titled " + jobmodel.Title, "/PublicationModel/Details/" + jobid, 6, user.UserProfileId);
                followPeersDB.Entry(user).State = EntityState.Modified;
                followPeersDB.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(jobmodel);
        }

        public void CreateUpdates(string message, string link, int type, int id)
        {
            string myname = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserProfileId == id);
            UserProfile myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == myname);
            Update record = new Update
            {
                Time = DateTime.Now,
                message = message,
                link = link,
                New = true,
                Own = true,
                type = type,
                owner = myprofile.UserProfileId
            };

            userprofile.Updates.Add(record); //add own update record

            string name = userprofile.UserName;
            IQueryable<string> custQuery = from cust in followPeersDB.Relationships where cust.personBName == name select cust.personAName;

            foreach (string personAname in custQuery)
            {
                Update record2 = new Update
                {
                    Time = DateTime.Now,
                    message = message,
                    link = link,
                    New = true,
                    Own = false,
                    type = type,
                    owner = userprofile.UserProfileId
                };

                UserProfile tempuserprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == personAname);

                tempuserprofile.Updates.Add(record2);
            } //add a new update record for the followers            
        }
        
        //
        // GET: /Default1/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Default1/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Default1/Delete/5

        public ViewResult Details(int id)
        {
            Jobs jobmodel = followPeersDB.Jobs.Find(id);

            string name = Membership.GetUser().UserName;
            UserProfile user = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            followPeersDB.Entry(user).State = EntityState.Modified;
            followPeersDB.SaveChanges();

            ViewBag.bookmarktag = false;
            // Check if a bookmark with these credentials exsists in the Db
            Bookmark bookmark = followPeersDB.Bookmarks.SingleOrDefault(b => b.itemID == id && b.userID == user.UserProfileId && b.bookmarkType == "Jobs");

            if (bookmark == null)
            {
                ViewBag.bookmarktag = true;
            }

            return View(jobmodel);
        }
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Default1/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
