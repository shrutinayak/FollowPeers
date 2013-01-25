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
    public class PatentModelController : Controller
    {
        private FollowPeersDBEntities db = new FollowPeersDBEntities();

        //
        // GET: /PatentModel/

        public ViewResult Index()
        {
            string name = Membership.GetUser().UserName;
            int userID;
            string rankTag1 = "default";
            string rankTag2 = "default";
            string rankTag3 = "default";

            // Get all possible patents
            UserProfile user = db.UserProfiles.SingleOrDefault(p => p.UserName == name);
            userID = user.UserProfileId;

            List<Specialization> special = user.Specializations.ToList();
            for (int loop = 0; loop < special.Count(); loop++)
            {
                String temp = special[loop].SpecializationName.ToString();
                if (loop == 0)
                    rankTag1 = temp;
                else if (loop == 1)
                    rankTag2 = temp;
                else if (loop == 2)
                    rankTag3 = temp;
            }
            

            var result = from n in db.PatentModels
                         orderby n.ID
                         select n;

            // Make it into a list
            List<PatentModel> resultlist = new List<PatentModel>();
            resultlist = result.ToList();

            // Get max of ID
            if(resultlist.Count != 0)
            {
                int maxID = resultlist[resultlist.Count() - 1].ID;

                for (int i = 0; i <= maxID; i++)
                {
                    PatentModel patentmodel = db.PatentModels.Find(i);
                    // If there is a hit then
                    if (patentmodel != null)
                    {
                        // Initialise ranking
                        patentmodel.Ranking = 0;
                        if (patentmodel.Keyword.Contains(rankTag1))
                        {
                            patentmodel.Ranking = patentmodel.Ranking + 1;
                        }
                        if (patentmodel.Title.Contains(rankTag1))
                        {
                            patentmodel.Ranking = patentmodel.Ranking + 1;
                        }
                        if (patentmodel.About.Contains(rankTag1))
                        {
                            patentmodel.Ranking = patentmodel.Ranking + 1;
                        }
                        if (patentmodel.UserProfile.Specializations.ToString().Contains(rankTag1))
                        {
                            patentmodel.Ranking = patentmodel.Ranking + 1;
                        }
                        if (patentmodel.Keyword.Contains(rankTag2))
                        {
                            patentmodel.Ranking = patentmodel.Ranking + 1;
                        }
                        if (patentmodel.Title.Contains(rankTag2))
                        {
                            patentmodel.Ranking = patentmodel.Ranking + 1;
                        }
                        if (patentmodel.About.Contains(rankTag2))
                        {
                            patentmodel.Ranking = patentmodel.Ranking + 1;
                        }
                        if (patentmodel.UserProfile.Specializations.ToString().Contains(rankTag2))
                        {
                            patentmodel.Ranking = patentmodel.Ranking + 1;
                        }
                        if (patentmodel.Keyword.Contains(rankTag3))
                        {
                            patentmodel.Ranking = patentmodel.Ranking + 1;
                        }
                        if (patentmodel.Title.Contains(rankTag3))
                        {
                            patentmodel.Ranking = patentmodel.Ranking + 1;
                        }
                        if (patentmodel.About.Contains(rankTag3))
                        {
                            patentmodel.Ranking = patentmodel.Ranking + 1;
                        }
                        if (patentmodel.UserProfile.Specializations.ToString().Contains(rankTag3))
                        {
                            patentmodel.Ranking = patentmodel.Ranking + 1;
                        }
                        //Put entry in db
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            
            var finalresult = from n in db.PatentModels
                              orderby n.Ranking descending
                              select n;
            

            return View(finalresult.ToList());

            //return View(db.PatentModels.ToList());
        }

        public ActionResult AjaxHandler(jQueryDataTableParamModel param)
        {
            return Json(new{
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = new List<string[]>() {
                    new string[] {"Test1", "A101", "2000", "2010", "Tier1"},
                    new string[] {"Test2", "A102", "2002", "2008", "Tier2"},
                    new string[] {"Test3", "A103", "2003", "2007", "Tier3"}
                    }
            },
        JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /PublicationModel/MyPublication

        public ViewResult MyPatent()
        {
            string name = Membership.GetUser().UserName;
            int userID;
            UserProfile user = db.UserProfiles.SingleOrDefault(p => p.UserName == name);
            userID = user.UserProfileId;
            var result = from n in db.PatentModels
                         where n.UserProfile.UserProfileId.Equals(userID)
                         orderby n.Title
                         orderby n.ViewCount descending
                         select n;

            return View(result.ToList());
        }
        //
        // GET: /PatentModel/Details/5

        public ViewResult Details(int id)
        {
            PatentModel patentmodel = db.PatentModels.Find(id);
            patentmodel.ViewCount = patentmodel.ViewCount + 1;
            
            string name = Membership.GetUser().UserName;
            UserProfile user = db.UserProfiles.SingleOrDefault(p => p.UserName == name);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.bookmarktag = false;
            // Check if a bookmark with these credentials exsists in the Db
            Bookmark bookmark = db.Bookmarks.SingleOrDefault(b => b.itemID == id && b.userID == user.UserProfileId && b.bookmarkType == "Patent");

            if (bookmark == null)
            {
                ViewBag.bookmarktag = true;
            }

            return View(patentmodel);
        }

        //
        // GET: /PatentModel/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /PatentModel/Create
        public void CreateUpdates(string message, string link, int type, int id)
        {
            string myname = Membership.GetUser().UserName;
            UserProfile userprofile = db.UserProfiles.SingleOrDefault(p => p.UserProfileId == id);
            UserProfile myprofile = db.UserProfiles.SingleOrDefault(p => p.UserName == myname);
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
            IQueryable<string> custQuery = from cust in db.Relationships where cust.personBName == name select cust.personAName;

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

                UserProfile tempuserprofile = db.UserProfiles.SingleOrDefault(p => p.UserName == personAname);

                tempuserprofile.Updates.Add(record2);
            } //add a new update record for the followers            
        }

        [HttpPost]
        public ActionResult Create(PatentModel patentmodel)
        {
            if (ModelState.IsValid)
            {
                string name = Membership.GetUser().UserName;
                UserProfile user = db.UserProfiles.SingleOrDefault(p => p.UserName == name);
                patentmodel.UserProfile = user;
                user.Patent.Add(patentmodel);
                //db.PatentModels.Add(patentmodel);
                //Create an update for patents
                CreateUpdates("Published a new patent titled " + patentmodel.Title, "/PatentModel/Details/" + patentmodel.ID, 7, user.UserProfileId);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyPatent");  
            }

            return View(patentmodel);
        }
        
        //
        // GET: /PatentModel/Edit/5
 
        public ActionResult Edit(int id)
        {
            PatentModel patentmodel = db.PatentModels.Find(id);
            return View(patentmodel);
        }

        //
        // POST: /PatentModel/Edit/5

        [HttpPost]
        public ActionResult Edit(PatentModel patentmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patentmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyPatent");
            }
            return View(patentmodel);
        }

        //
        // GET: /PatentModel/Delete/5
 
        public ActionResult Delete(int id)
        {
            PatentModel patentmodel = db.PatentModels.Find(id);
            return View(patentmodel);
        }

        //
        // POST: /PatentModel/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            PatentModel patentmodel = db.PatentModels.Find(id);
            db.PatentModels.Remove(patentmodel);
            db.SaveChanges();
            return RedirectToAction("MyPatent");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult Search(string searchstring)
        {
            //Added by Ritesh, bug fix on search
            ViewData["keywords"] = searchstring;
            //

            string name = Membership.GetUser().UserName;
            UserProfile userprofile = db.UserProfiles.SingleOrDefault(p => p.UserName == name);
            if (searchstring.Count() != 0)
            {
                IEnumerable<PatentModel> result = from PatentModel in db.PatentModels
                                                  where (PatentModel.About.Contains(searchstring) || PatentModel.Title.Contains(searchstring) || PatentModel.Keyword.Contains(searchstring))
                                                  orderby PatentModel.Title
                                                  select PatentModel;
                ViewBag.searchstring = "Search Patent Results for " + searchstring;
                return View(result);
            }
            else
            {
                IEnumerable<PatentModel> result = from PatentModel in db.PatentModels
                                                  orderby PatentModel.Title
                                                  select PatentModel;
                ViewBag.searchstring = "Showing All Patents";
                return View(result);
            }
        }

        internal List<Country> FindCountry(string searchText, int maxResults)
        {
            var result = from n in db.Countries
                         where n.Name.Contains(searchText)
                         orderby n.Name
                         select n;

            return result.Take(maxResults).ToList();

        }
        [HttpPost]
        public JsonResult FindCountryNames(string searchText, int maxResults)
        {
            var result = FindCountry(searchText, maxResults);
            return Json(result);
        }

        [HttpPost]
        public ActionResult AddBookmark(string ID)
        {
            int id = Convert.ToInt16(ID);
            // Get the user name of the current logged in user
            string name = Membership.GetUser().UserName;
            // Get the entire userprofile associated with this user
            UserProfile user = db.UserProfiles.SingleOrDefault(p => p.UserName == name);

            // Check if a bookmark with these credentials exsists in the Db
            Bookmark bookmark = db.Bookmarks.SingleOrDefault(b => b.itemID == id && b.userID == user.UserProfileId && b.bookmarkType == "Patent");

            if (bookmark == null)
            {
                // No Bookmark exsists,  thereforre add this bookmark into the Db
                Bookmark tempBookmark = new Bookmark();
                tempBookmark.itemID = id;
                tempBookmark.bookmarkType = "Patent";
                tempBookmark.userID = user.UserProfileId;
                db.Bookmarks.Add(tempBookmark);
                db.SaveChanges();

                //Adding a notification item to the owner of the patent
                PatentModel patent = db.PatentModels.SingleOrDefault(b => b.ID == tempBookmark.itemID);
                UserProfile updateowner = db.UserProfiles.SingleOrDefault(p => p.UserProfileId == patent.UserProfile.UserProfileId);
                string patentDescription = patent.Title;
                Notification newnoti = new Notification
                {
                    message = user.FirstName + " bookmarked your patent : " + patentDescription,
                    link = "/Profile/Index/" + user.UserProfileId,
                    New = true,
                    imagelink = user.PhotoUrl,
                };

                updateowner.Notifications.Add(newnoti);
                db.Entry(updateowner).State = EntityState.Modified;
                db.SaveChanges();
            }

            

            return RedirectToAction("Index", "PublicationModel");
        }


        [HttpPost]
        public ActionResult DeleteBookmark(string ID)
        {
            int id = Convert.ToInt16(ID);
            // Get the user name of the current logged in user
            string name = Membership.GetUser().UserName;
            // Get the entire userprofile associated with this user
            UserProfile user = db.UserProfiles.SingleOrDefault(p => p.UserName == name);

            // Check if a bookmark with these credentials exsists in the Db
            Bookmark bookmark = db.Bookmarks.SingleOrDefault(b => b.itemID == id && b.userID == user.UserProfileId && b.bookmarkType == "Patent");
            db.Bookmarks.Remove(bookmark);
            db.SaveChanges();
            return RedirectToAction("Index", "PublicationModel");
        }

    }
}