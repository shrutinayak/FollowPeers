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
    public class PublicationModelController : Controller
    {
        private FollowPeersDBEntities followPeersDB = new FollowPeersDBEntities();

        //
        // GET: /PublicationModel/

        public ViewResult Index()
        {
            string name = Membership.GetUser().UserName;
            int userID;
            UserProfile user = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            userID = user.UserProfileId;
            var result = from n in followPeersDB.PublicationModels
                         orderby n.viewCount descending
                         select n;
            
            return View(result.ToList());
            //return View(followPeersDB.PublicationModels.ToList());
        }

        //
        // GET: /PublicationModel/MyPublication

       /* public ViewResult MyPublication()
        {
            string name = Membership.GetUser().UserName;
            int userID;
            UserProfile user = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            userID = user.UserProfileId;
            var result = from n in followPeersDB.PublicationModels
                         where n.ownerID.Equals(userID)
                         orderby n.title
                         select n;

            return View(result.ToList());
        }*/


        //
        // GET: /PublicationModel/Details/5

        public ViewResult Details(int id)
        {
            PublicationModel publicationmodel = followPeersDB.PublicationModels.Find(id);
            publicationmodel.viewCount = publicationmodel.viewCount + 1;

            string name = Membership.GetUser().UserName;
            UserProfile user = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            followPeersDB.Entry(user).State = EntityState.Modified;
            followPeersDB.SaveChanges();

            ViewBag.bookmarktag = false;
            // Check if a bookmark with these credentials exsists in the Db
            Bookmark bookmark = followPeersDB.Bookmarks.SingleOrDefault(b => b.itemID == id && b.userID == user.UserProfileId && b.bookmarkType == "Publication");

            if (bookmark == null)
            {
                ViewBag.bookmarktag = true;
            }

            return View(publicationmodel);
        }

        //
        // GET: /PublicationModel/Create

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
        // POST: /PublicationModel/Create

        [HttpPost]
        public ActionResult Create(PublicationModel publicationmodel)
        {
            if (ModelState.IsValid)
            {
                if (publicationmodel.author == null)
                {
                    publicationmodel.author = "default";
                }
                if (publicationmodel.conference == null)
                {
                    publicationmodel.conference = "default";
                }
                if (publicationmodel.description == null)
                {
                    publicationmodel.description = "default";
                }
                if (publicationmodel.issue == null)
                {
                    publicationmodel.issue = "default";
                }
                if (publicationmodel.journal == null)
                {
                    publicationmodel.journal = "default";
                }
                if (publicationmodel.keyword == null)
                {
                    publicationmodel.keyword = "default";
                }
                if (publicationmodel.page == null)
                {
                    publicationmodel.page = "default";
                }
                if (publicationmodel.publisher == null)
                {
                    publicationmodel.publisher = "default";
                }
                if (publicationmodel.referenceID == null)
                {
                    publicationmodel.referenceID = "Singapore";
                }
                if (publicationmodel.status == null)
                {
                    publicationmodel.status = "default";
                }
                if (publicationmodel.title == null)
                {
                    publicationmodel.title = "default";
                } 
                if (publicationmodel.type == null)
                {
                    publicationmodel.type = "default";
                } 
                if (publicationmodel.university == null)
                {
                    publicationmodel.university = "default";
                }
                if (publicationmodel.volume == null)
                {
                    publicationmodel.volume = "default";
                }
                if (publicationmodel.year == null)
                {
                    publicationmodel.year = 2012;
                }
                if (publicationmodel.viewCount == 0)
                {
                    publicationmodel.viewCount = 0;
                }
                if (publicationmodel.specialisation == null)
                {
                    publicationmodel.specialisation = "Physics";
                }
                publicationmodel.timestamp = DateTime.Now.ToString();

                string name = Membership.GetUser().UserName;
                UserProfile user = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
                publicationmodel.UserProfile = user;
                user.Publication.Add(publicationmodel);

                int publicationmodelid = followPeersDB.PublicationModels.Count() + 1;
                CreateUpdates("Published a new publication titled " + publicationmodel.title, "/PublicationModel/Details/" + publicationmodelid, 6, user.UserProfileId);

                //followPeersDB.PublicationModels.Add(publicationmodel);
                followPeersDB.Entry(user).State = EntityState.Modified;
                followPeersDB.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(publicationmodel);
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

        internal List<Country> FindCountry(string searchText, int maxResults)
        {
            var result = from n in followPeersDB.Countries
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

        internal List<Journal> findJournal(string searchText, int maxResults)
        {
            var result = from n in followPeersDB.Journals
                         where n.Name.Contains(searchText)
                         orderby n.Name
                         select n;
            return result.Take(maxResults).ToList();
        }

        [HttpPost]
        public JsonResult findJournalNames(string searchText, int maxResults)
        {
            var result = findJournal(searchText, maxResults);
            return Json(result);
        }

        internal List<Publisher> findPublisher(string searchText, int maxResults)
        {
            var result = from n in followPeersDB.Publishers
                         where n.Name.Contains(searchText)
                         orderby n.Name
                         select n;
            return result.Take(maxResults).ToList();
        }

        [HttpPost]
        public JsonResult findPublisherNames(string searchText, int maxResults)
        {
            var result = findPublisher(searchText, maxResults);
            return Json(result);
        }

        internal List<Conference> findConference(string searchText, int maxResults)
        {
            var result = from n in followPeersDB.Conferences
                         where n.Name.Contains(searchText)
                         orderby n.Name
                         select n;
            return result.Take(maxResults).ToList();
        }

        [HttpPost]
        public JsonResult findConferenceNames(string searchText, int maxResults)
        {
            var result = findConference(searchText, maxResults);
            return Json(result);
        }

        internal List<University> findUniversity(string searchText, int maxResults)
        {
            var result = from n in followPeersDB.Universities
                         where n.Name.Contains(searchText)
                         orderby n.Name
                         select n;
            return result.Take(maxResults).ToList();
        }

        [HttpPost]
        public JsonResult findUniversityNames(string searchText, int maxResults)
        {
            var result = findUniversity(searchText, maxResults);
            return Json(result);
        }


        //
        // GET: /PublicationModel/Edit/5

        public ActionResult Edit(int id)
        {
            PublicationModel publicationmodel = followPeersDB.PublicationModels.Find(id);
            return View(publicationmodel);
        }

        //
        // POST: /PublicationModel/Edit/5

        [HttpPost]
        public ActionResult Edit(PublicationModel publicationmodel)
        {
            if (ModelState.IsValid)
            {
                followPeersDB.Entry(publicationmodel).State = EntityState.Modified;
                followPeersDB.SaveChanges();
                return RedirectToAction("MyPublication");
            }
            return View(publicationmodel);
        }

        //
        // GET: /PublicationModel/Delete/5

        public ActionResult Delete(int id)
        {
            PublicationModel publicationmodel = followPeersDB.PublicationModels.Find(id);
            return View(publicationmodel);
        }

        //
        // POST: /PublicationModel/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            PublicationModel publicationmodel = followPeersDB.PublicationModels.Find(id);
            followPeersDB.PublicationModels.Remove(publicationmodel);
            followPeersDB.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            followPeersDB.Dispose();
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult Search(string searchstring)
        {
            //Added by Ritesh, bug fix on search
            ViewData["keywords"] = searchstring;
            //

            string name = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            if (searchstring.Count() != 0)
            {
                IEnumerable<PublicationModel> result = from PublicationModel in followPeersDB.PublicationModels
                                                       where (PublicationModel.title.Contains(searchstring) ||
                                                       PublicationModel.description.Contains(searchstring) ||
                                                       PublicationModel.keyword.Contains(searchstring)
                                                       )
                                                       orderby PublicationModel.title
                                                       select PublicationModel;
                ViewBag.searchstring = "Search Results for " + searchstring;
                return View(result);
            }
            else
            {
                IEnumerable<PublicationModel> result = from PublicationModel in followPeersDB.PublicationModels
                                                       orderby PublicationModel.title
                                                       select PublicationModel;
                ViewBag.searchstring = "Showing All Publication";
                return View(result);
            }
        }

        [HttpPost]
        public ActionResult AddBookmark(string ID)
        {
            int id = Convert.ToInt16(ID);
            // Get the user name of the current logged in user
            string name = Membership.GetUser().UserName;
            // Get the entire userprofile associated with this user
            UserProfile user = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);

            // Check if a bookmark with these credentials exsists in the Db
            Bookmark bookmark = followPeersDB.Bookmarks.SingleOrDefault(b => b.itemID == id && b.userID == user.UserProfileId && b.bookmarkType == "Publication");

            if (bookmark == null)
            {
                // No Bookmark exsists,  thereforre add this bookmark into the Db
                Bookmark tempBookmark = new Bookmark();
                tempBookmark.itemID = id;
                tempBookmark.bookmarkType = "Publication";
                tempBookmark.userID = user.UserProfileId;
                followPeersDB.Bookmarks.Add(tempBookmark);
                followPeersDB.SaveChanges(); 
                //Adding a notification item to the owner of the publication
                PublicationModel book = followPeersDB.PublicationModels.SingleOrDefault(b => b.publicationID == tempBookmark.itemID);
                UserProfile updateowner = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserProfileId == book.ownerID);
                string bookdescription = book.title;
                Notification newnoti = new Notification
                {
                    message = user.FirstName + " bookmarked your publication : " + bookdescription,
                    link = "/Profile/Index/" + user.UserProfileId,
                    New = true,
                    imagelink = user.PhotoUrl,
                };

                updateowner.Notifications.Add(newnoti);
                followPeersDB.Entry(updateowner).State = EntityState.Modified;
                followPeersDB.SaveChanges();
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
            UserProfile user = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);

            // Check if a bookmark with these credentials exsists in the Db
            Bookmark bookmark = followPeersDB.Bookmarks.SingleOrDefault(b => b.itemID == id && b.userID == user.UserProfileId && b.bookmarkType == "Publication");
                followPeersDB.Bookmarks.Remove(bookmark);
                followPeersDB.SaveChanges();
                return RedirectToAction("Index", "PublicationModel");
        }

    }
}