using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FollowPeers.Models;
using System.Web.Security;
using System.Data;
using System.Data.Entity;
using System.Net.Mail;
using System.Net;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Web.Helpers;
using System.IO;

namespace FollowPeers.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        //
        // GET: /Profile/
        FollowPeersDBEntities followPeersDB = new FollowPeersDBEntities();
        static UserProfile myprofile;
        public ActionResult Index(int id)
        {
            string name = Membership.GetUser().UserName;
            UserProfile viewerprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserProfileId == id);
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);

            if (myprofile == null) //if the database stores a wrong info about name
            {
                name = " " + name;
                myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
                myprofile.UserName.Trim();
                followPeersDB.SaveChanges();
            }

            if (viewerprofile.firsttime == true)
            {
                return RedirectToAction("Edit", "Profile", new { id = viewerprofile.UserProfileId });

            }
            else
            {
                ViewBag.Name = viewerprofile.UserName;
                if ((followPeersDB.Relationships.SingleOrDefault(p => p.personAName == name && p.personBName == viewerprofile.UserName) != null))//already followed
                    ViewBag.Alreadyfollowed = 1;
                else
                    ViewBag.Alreadyfollowed = 0;

                return View(viewerprofile);
            }

        }
        [ChildActionOnly]
        public ActionResult JobRecommendation()
        {
            string name = Membership.GetUser().UserName;
            UserProfile myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            string specializationname = " "; IEnumerable<Jobs> tempresult = null;
            if (myprofile.Specializations.Count() != 0)
            {
                specializationname = myprofile.Specializations.ElementAt(0).SpecializationName;
                Specialization spec = followPeersDB.Specializations.First(p => p.SpecializationName.Contains(specializationname));
                tempresult = from j in followPeersDB.Jobs
                             where ((j.Country == myprofile.Country) && (j.ownerID != myprofile.UserProfileId))
                             orderby j.Title
                             select j;

                List<Jobs> temp = new List<Jobs>();
                foreach (var r in tempresult)
                {
                   /* if (r.Specializations.Contains(spec))
                    {
                        temp.Add(r);
                    }*/
                }
                if (temp.Count() > 2) return PartialView(temp);
            }

            tempresult = from j in followPeersDB.Jobs
                         where ((j.Country == myprofile.Country) && (j.ownerID != myprofile.UserProfileId))
                         orderby j.Title
                         select j;


            return PartialView(tempresult);

        }

        public ActionResult ViewRecommendedJobs()
        {
            string name = Membership.GetUser().UserName;
            UserProfile myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            string specializationname = " "; IEnumerable<Jobs> tempresult = null;
            if (myprofile.Specializations.Count() != 0)
            {
                specializationname = myprofile.Specializations.ElementAt(0).SpecializationName;
                Specialization spec = followPeersDB.Specializations.First(p => p.SpecializationName.Contains(specializationname));
                tempresult = from j in followPeersDB.Jobs
                             where ((j.Country == myprofile.Country) && (j.ownerID != myprofile.UserProfileId))
                             orderby j.Title
                             select j;

                List<Jobs> temp = new List<Jobs>();
                foreach (var r in tempresult)
                {
                    /*if (r.Specializations.Contains(spec))
                    {
                        temp.Add(r);
                    }*/
                }
                if (temp.Count() > 2) return View(temp);
            }

            tempresult = from j in followPeersDB.Jobs
                         where ((j.Country == myprofile.Country) && (j.ownerID != myprofile.UserProfileId))
                         orderby j.Title
                         select j;
            return View(tempresult);
        }


        [ChildActionOnly]
        public ActionResult Following(int id)
        {
            //string name = Membership.GetUser().UserName;

            UserProfile temp = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserProfileId == id);
            string name = temp.UserName;
            List<UserProfile> result = new List<UserProfile>();

            IQueryable<string> custQuery = from cust in followPeersDB.Relationships where cust.personAName == name select cust.personBName;

            foreach (string personBname in custQuery)
            {
                result.Add(followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == personBname));
            }

            ViewData["link"] = temp.UserName;
            return PartialView(result);
        }

        [ChildActionOnly]
        public ActionResult FollowedBy(int id)
        {
            //string name = Membership.GetUser().UserName;

            UserProfile temp = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserProfileId == id);
            string name = temp.UserName;
            List<UserProfile> result = new List<UserProfile>();

            IQueryable<string> custQuery = from cust in followPeersDB.Relationships where cust.personBName == name select cust.personAName;

            foreach (string personAname in custQuery)
            {
                result.Add(followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == personAname));
            }
            ViewData["link"] = temp.UserName;

            return PartialView(result);
        }

        [HttpGet]
        public ActionResult FollowedByList(string name)
        {
            /*List<UserProfile> result = new List<UserProfile>();
            string myname = Membership.GetUser().UserName;
            IQueryable<string> custQuery = from cust in followPeersDB.Relationships where cust.personBName == name select cust.personAName;

            foreach (string personAname in custQuery)
            {
                result.Add(followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == personAname));
            }
            foreach (var u in result)
            {
                string temp = u.UserName;
                if ((followPeersDB.Relationships.SingleOrDefault(p => p.personAName == myname && p.personBName == u.UserName) != null))//already followed
                    ViewData[temp] = "1";
                else
                    ViewData[temp] = "0";
            }
            ViewData["link"] = name;
            if (name == myname)
            { ViewData["myself"] = 1; }
            else
            { ViewData["myself"] = 0; }

            return View(result);*/
            Session.Add("username", name);
            return RedirectToAction("Index", "FollowedBy", new { sort = 1 });
        }

        [HttpGet]
        public ActionResult FollowingList(string name)
        {
           /* List<UserProfile> result = new List<UserProfile>();
            string myname = Membership.GetUser().UserName;
            IQueryable<string> custQuery = from cust in followPeersDB.Relationships where cust.personAName == name select cust.personBName;

            foreach (string personAname in custQuery)
            {
                result.Add(followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == personAname));
            }
            foreach (var u in result)
            {
                string temp = u.UserName;
                if ((followPeersDB.Relationships.SingleOrDefault(p => p.personAName == myname && p.personBName == u.UserName) != null))//already followed
                    ViewData[temp] = "1";
                else
                    ViewData[temp] = "0";
            }
            ViewData["link"] = name;
            if (name == myname)
            { ViewData["myself"] = 1; }
            else
            { ViewData["myself"] = 0; }

            return View(result);*/
            Session.Add("username", name);
            return RedirectToAction("Index", "FollowedBy", new { sort = 1 });
        }
        [ChildActionOnly]
        public ActionResult FollowPeopleRecommendation()
        {

            string myname = Membership.GetUser().UserName;
            FollowPeers.Models.UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == myname);
            string organization = userprofile.Organization;
            string department = userprofile.Departments;
            IEnumerable<UserProfile> result;
            List<UserProfile> resultlist = null;
            result = from UserProfile in followPeersDB.UserProfiles
                     where (UserProfile.Organization.Contains(organization) && UserProfile.activated == true)
                     orderby UserProfile.UserName
                     select UserProfile;
            if (result.Count() < 10)
            {
                result = from UserProfile in followPeersDB.UserProfiles
                         where ((UserProfile.Organization.Contains(organization) || (UserProfile.Departments.Contains(department))) && UserProfile.activated == true)
                         orderby UserProfile.UserName
                         select UserProfile;
            }

            if (result.Count() < 10)
            {
                result = from UserProfile in followPeersDB.UserProfiles
                         orderby UserProfile.UserName
                         select UserProfile;
            }
            resultlist = result.ToList();
            foreach (var r in result)
            {
                if ((followPeersDB.Relationships.SingleOrDefault(p => p.personAName == userprofile.UserName && p.personBName == r.UserName) != null)) //already followed
                {
                    resultlist.Remove(r);
                }
                if (r.UserProfileId == userprofile.UserProfileId)
                    resultlist.Remove(r);
            }

            return PartialView(resultlist);

        }
        [ChildActionOnly]
        public ActionResult ProgressTracker()
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            ViewBag.Percent = (myprofile.SignUpProgress * 100).ToString();
            if (myprofile.Organization == null) { ViewBag.Text = "Update Basic Info"; ViewBag.Link = Server.HtmlEncode("/Profile/Edit"); }
            else if (myprofile.PhotoUrl == "/Content/TempImages/default.jpg") { ViewBag.Text = "Upload Profile Photo"; ViewBag.Link = Server.HtmlEncode("/Profile/UploadPhoto"); }
            else if (myprofile.Specializations.Count() == 0) { ViewBag.Text = "Update Research Interests"; ViewBag.Link = Server.HtmlEncode("/Profile/EditResearch"); }
            else if (myprofile.Educations.Count() == 0) { ViewBag.Text = "Update Education History"; ViewBag.Link = Server.HtmlEncode("/Profile/EditEducation"); }
            else if (myprofile.Contact.Email == null) { ViewBag.Text = "Update Contact Info"; ViewBag.Link = Server.HtmlEncode("/Profile/EditContact"); }

            return PartialView();
        }

        /// <summary>
        /// Bookmark Publication
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult BookmarkPublication()
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);

            int numberOfBookmarks = myprofile.PublicationBookmark.Count();
            List<int> bookmarkID = myprofile.PublicationBookmark.ToList();
            ViewBag.infopresent = false;

            if (numberOfBookmarks == 1)
            {
                var result = from n in followPeersDB.PublicationModels
                             where n.publicationID.Equals(bookmarkID[0])
                             orderby n.title
                             select n;
                ViewBag.infopresent = true;
                return PartialView(result.ToList());
            }
            if (numberOfBookmarks == 2)
            {
                var result = from n in followPeersDB.PublicationModels
                             where (n.publicationID.Equals(bookmarkID[0]) || n.publicationID.Equals(bookmarkID[1]))
                             orderby n.title
                             select n;
                ViewBag.infopresent = true;
                return PartialView(result.ToList());
            }
            if (numberOfBookmarks == 3)
            {
                var result = from n in followPeersDB.PublicationModels
                             where (n.publicationID.Equals(bookmarkID[0]) || n.publicationID.Equals(bookmarkID[1]) || n.publicationID.Equals(bookmarkID[2]))
                             orderby n.title
                             select n;
                ViewBag.infopresent = true;
                return PartialView(result.ToList());
            }
            if (numberOfBookmarks == 4)
            {
                var result = from n in followPeersDB.PublicationModels
                             where (n.publicationID.Equals(bookmarkID[0]) || n.publicationID.Equals(bookmarkID[1]) || n.publicationID.Equals(bookmarkID[2]) || n.publicationID.Equals(bookmarkID[3]))
                             orderby n.title
                             select n;
                ViewBag.infopresent = true;
                return PartialView(result.ToList());
            }
            if (numberOfBookmarks == 5)
            {
                var result = from n in followPeersDB.PublicationModels
                             where (n.publicationID.Equals(bookmarkID[0]) || n.publicationID.Equals(bookmarkID[1]) || n.publicationID.Equals(bookmarkID[2]) || n.publicationID.Equals(bookmarkID[3]) || n.publicationID.Equals(bookmarkID[4]))
                             orderby n.title
                             select n;
                ViewBag.infopresent = true;
                return PartialView(result.ToList());
            }
            return PartialView();
        }



        public ActionResult Edit(string message)
        {
            if (message != null) ViewData["message"] = message;
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            return View(myprofile);
        }

        public ActionResult PostJob(string message)
        {
            if (message != null) ViewData["message"] = message;
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            return View(myprofile);
        }

        public ActionResult EditResearch(string message)
        {
            if (message != null) ViewData["message"] = message;
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            return View(myprofile);
        }
        public ActionResult EditStudents(string message)
        {
            if (message != null) ViewData["message"] = message;
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            return View(myprofile);
        }

        public ActionResult EditEducation(string message)
        {
            if (message != null) ViewData["message"] = message;
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            ViewBag.CountryId = new SelectList(followPeersDB.Countries, "CountryId", "Name");
            return View(myprofile);
        }

        public ActionResult EditPortfolio(string message)
        {
            if (message != null) ViewData["message"] = message;
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            ViewBag.CountryId = new SelectList(followPeersDB.Countries, "CountryId", "Name");
            return View(myprofile);
        }

        public ActionResult EditContact(string message)
        {
            if (message != null) ViewData["message"] = message;
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            return View(myprofile);
        }
        public ActionResult UploadPhoto()
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            return View(myprofile);
        }
        [HttpPost]
        public ActionResult UploadPhoto(FormCollection formCollection)
        {
            string name = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            bool toadd = false;
            if (userprofile.PhotoUrl == "/Content/TempImages/default.jpg") toadd = true;
            var image = WebImage.GetImageFromRequest();
            if (image != null)
            {
                if (image.Width > 500)
                {
                    image.Resize(500, ((500 * image.Height) / image.Width));
                }

                var filename = Path.GetFileName(image.FileName);
                string fullfilename = userprofile.UserProfileId.ToString() + filename;
                fullfilename.Replace("  ", string.Empty);
                image.Save(Path.Combine("../Content/TempImages", fullfilename));
                filename = Path.Combine("~/Content/TempImages", fullfilename);
                if (userprofile.SignUpProgress + 0.2F <= 1.0F && toadd == true) userprofile.SignUpProgress += 0.20F;

                userprofile.PhotoUrl = Url.Content(filename);
                if (toadd == false) //this means the user is NOT editing the specialization records for the first time.. thus need to create an update record
                {
                    CreateUpdates("New photo uploaded.", "/Profile/Index/" + userprofile.UserProfileId, 1, userprofile.UserProfileId); //CreateUpdates(message,link,type)
                }
                followPeersDB.Entry(userprofile).State = EntityState.Modified;
                followPeersDB.SaveChanges();
                if (userprofile.Specializations.Count() == 0) return RedirectToAction("EditResearch", "Profile");

                return View("Index", new { id = userprofile.UserProfileId });

            }

            return View(userprofile);
        }


        //
        // POST: /Profile/Edit/5
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(FormCollection formCollection, int[] Specialization)
        {
            string name = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            bool toadd = false;
            if (userprofile.Organization == null) toadd = true;
            // if (ModelState.IsValid && userprofile.FirstName != null && userprofile.LastName != null)

            if (TryUpdateModel(userprofile, "", new string[] { "Country", "FirstName", "LastName", "Gender", "Status", "Birthday", "Organization", "AboutMe", "Departments" }, new string[] { "PhotoUrl", "Status", "StatusMessage", "Specializations", "Educations", "Contact" }))
            {
                //if a user adds a new organization
                if ((followPeersDB.Organizations.SingleOrDefault(p => p.Name == userprofile.Organization)) == null)
                {
                    followPeersDB.Organizations.Add(new Organization { Name = userprofile.Organization });
                }

                //if a user adds a new Department
                if ((followPeersDB.Departments.SingleOrDefault(p => p.Name == userprofile.Departments)) == null)
                {
                    followPeersDB.Departments.Add(new Department { Name = userprofile.Departments });
                }

                UpdateSpecializations(Specialization, userprofile);

                userprofile.firsttime = false;

                if (userprofile.SignUpProgress + 0.2F <= 1.1F && toadd == true) userprofile.SignUpProgress += 0.20F;
                if (toadd == false) //this means the user is NOT editing the specialization records for the first time.. thus need to create an update record
                {
                    CreateUpdates("Profile information updated.", "/Profile/Index/" + userprofile.UserProfileId, 1, userprofile.UserProfileId); //CreateUpdates(message,link,type)
                }
                followPeersDB.Entry(userprofile).State = EntityState.Modified;
                followPeersDB.SaveChanges();
                if (userprofile.PhotoUrl == "/Content/TempImages/default.jpg") return RedirectToAction("UploadPhoto", "Profile");

                return RedirectToAction("Edit", "Profile", new { message = "Successfully Updated" });
            }

            if (userprofile.FirstName == null) ModelState.AddModelError("", "First Name cannot be left blank.");
            if (userprofile.LastName == null) ModelState.AddModelError("", "Last Name cannot be left blank.");
            ModelState.AddModelError("", "Update Failed");


            return View(userprofile);


        }

        [HttpPost]
        public ActionResult EditResearch(FormCollection formCollection, int[] Specialization)
        {
            string name = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            bool toadd = false;
            if (userprofile.Specializations.Count() == 0) toadd = true;

            if (TryUpdateModel(userprofile, "", null, new string[] { "Specializations" }))
            {
                //if a user adds a new organization
                if ((followPeersDB.Organizations.SingleOrDefault(p => p.Name == userprofile.Organization)) == null)
                {
                    followPeersDB.Organizations.Add(new Organization { Name = userprofile.Organization });
                }
                UpdateSpecializations(Specialization, userprofile);

                userprofile.firsttime = false;
                if (userprofile.SignUpProgress + 0.2F <= 1.1F && toadd == true) userprofile.SignUpProgress += 0.20F;
                if (toadd == false) //this means the user is NOT editing the specialization records for the first time.. thus need to create an update record
                {
                    CreateUpdates("Research interest updated.", "/Profile/Index/" + userprofile.UserProfileId, 1, userprofile.UserProfileId); //CreateUpdates(message,link,type)
                }


                followPeersDB.Entry(userprofile).State = EntityState.Modified;
                followPeersDB.SaveChanges();
                if (myprofile.Educations.Count() == 0) return RedirectToAction("EditEducation", "Profile");
                return RedirectToAction("EditResearch", "Profile", new { message = "Successfully Updated" });
            }

            if (userprofile.FirstName == null) ModelState.AddModelError("", "First Name cannot be left blank.");
            if (userprofile.LastName == null) ModelState.AddModelError("", "Last Name cannot be left blank.");

            return View(userprofile);

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
                if (type == 5) { record2.type = 4; record2.owner = myprofile.UserProfileId; record2.Own = true; }
                UserProfile tempuserprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == personAname);
                if (record2.owner != tempuserprofile.UserProfileId && type == 5) { record2.Own = false; }
                tempuserprofile.Updates.Add(record2);
            } //add a new update record for the followers            
        }

        

        [HttpPost]
        public ActionResult EditEducation(FormCollection formCollection, string[] Organization, string[] startYear, string[] endYear, string[] Degree, string[] Country)
        {
            string name = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            bool toadd = false;
            if (userprofile.Educations.Count() == 0) toadd = true;

            if (TryUpdateModel(userprofile, "", null, new string[] { "Organization", "Specializations", "FirstName", "LastName", "Profession", "Gender", "Status" }))
            {

                UpdateEducation(Organization, startYear, endYear, Degree, Country, userprofile);

                userprofile.firsttime = false;
                if (userprofile.SignUpProgress + 0.2F <= 1.1F) userprofile.SignUpProgress += 0.20F;
                // userprofile.SignUpProgress = 1.0F;
                if (toadd == false) //this means the user is NOT editing the specialization records for the first time.. thus need to create an update record
                {
                    CreateUpdates("Education updated.", "/Profile/Index/" + userprofile.UserProfileId, 1, userprofile.UserProfileId); //CreateUpdates(message,link,type)
                }
                followPeersDB.Entry(userprofile).State = EntityState.Modified;
                followPeersDB.SaveChanges();
                if (myprofile.Contact == null) return RedirectToAction("EditContact", "Profile");

                return RedirectToAction("EditEducation", "Profile", new { message = "Successfully Updated" });
            }

            return View(userprofile);


        }
        private void UpdateEducation(string[] Organization, string[] startYear, string[] endYear, string[] Degree, string[] Country, UserProfile userprofile)
        {
            //if new organization, add to the DB
            foreach (var org in Organization)
            {
                if ((followPeersDB.Organizations.SingleOrDefault(p => p.Name == org)) == null)
                {
                    followPeersDB.Organizations.Add(new Organization { Name = org });
                }

            }
            int count = userprofile.Educations.Count();
            for (int i = 0; i < count; i++)
            {
                Education tempEdu = userprofile.Educations.ElementAt(0);
                followPeersDB.Educations.Remove(tempEdu);
                //       userprofile.Educations.Remove(tempEdu);
            }
            for (int i = 0; i < Organization.Count(); i++)
            {
                if (Organization != null)
                {
                    string tempUni = Organization.ElementAt(i);
                    string tempStart = startYear.ElementAt(i);
                    string tempEnd = endYear.ElementAt(i);
                    string tempDegree = Degree.ElementAt(i);
                    string tempCountry = Country.ElementAt(i);
                    Education tempEdu = new Education
                    {
                        UniversityName = tempUni,
                        startYear = Convert.ToInt16(tempStart),
                        endYear = Convert.ToInt16(tempEnd),
                        Degree = tempDegree,
                        country = tempCountry
                    };
                    userprofile.Educations.Add(tempEdu);
                    tempEdu.UserProfile = userprofile;
                }
            }
        }

        [HttpPost]
        public ActionResult EditPortfolio(FormCollection formCollection, string[] Name, string[] Field, string[] Country, string[] Year, string[] Status, string[] Website, string[] MoreInfo)
        {
            string name = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);

            if (TryUpdateModel(userprofile, "", null, new string[] { "Organization", "Educations", "Specializations", "FirstName", "LastName", "Profession", "Gender", "Status" }))
            {
                UpdatePortfolio(Name, Field, Country, Year, Status, Website, MoreInfo, userprofile);
                followPeersDB.Entry(userprofile).State = EntityState.Modified;
                followPeersDB.SaveChanges();
                return RedirectToAction("EditPortfolio", "Profile", new { message = "Successfully Updated" });
            }
            return View(userprofile);
        }
        private void UpdatePortfolio(string[] Name, string[] Field, string[] Country, string[] Year, string[] Status, string[] Website, string[] MoreInfo, UserProfile userprofile)
        {
            int count = userprofile.Portfolios.Count();
            for (int i = 0; i < count; i++)
            {
                Portfolio tempCompany = userprofile.Portfolios.ElementAt(0);
                followPeersDB.Portfolios.Remove(tempCompany);
                //       userprofile.Educations.Remove(tempEdu);
            }
            if (Name != null)
            {
                for (int i = 0; i < Name.Count(); i++)
                {

                    string tempName = Name.ElementAt(i);
                    string tempField = Field.ElementAt(i);
                    string tempCountry = Country.ElementAt(i);
                    string tempYear = Year.ElementAt(i);
                    string tempStatus = Status.ElementAt(i);
                    string tempWebsite = Website.ElementAt(i);
                    string tempMoreInfo = MoreInfo.ElementAt(i);
                    Portfolio tempPort = new Portfolio
                    {
                        Name = tempName,
                        BusinessField = tempField,
                        Country = tempCountry,
                        Year = Convert.ToInt16(tempYear),
                        Status = tempStatus,
                        Website = tempWebsite,
                        MoreInfo = tempMoreInfo
                    };
                    userprofile.Portfolios.Add(tempPort);
                    tempPort.UserProfile = userprofile;
                }
            }
        }


        [HttpPost]
        public ActionResult EditStudents(FormCollection formCollection, string[] Name, string[] Type, string[] StartYear, string[] EndYear, string[] About, string[] Link)
        {
            string name = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            bool toadd = false;
            if (userprofile.Students.Count() == 0) toadd = true;

            // if (TryUpdateModel(userprofile, "", null, new string[] { "Students","Education","Organization", "Specializations", "FirstName", "LastName", "Profession", "Gender", "Status" }))
            {

                UpdateStudent(Name, Type, StartYear, EndYear, About, Link, userprofile);

                userprofile.firsttime = false;
                // userprofile.SignUpProgress = 1.0F;
                // if (toadd == false) //this means the user is NOT editing the specialization records for the first time.. thus need to create an update record
                {
                    //       CreateUpdates("Education updated.", "/Profile/Index/" + userprofile.UserProfileId, 1); //CreateUpdates(message,link,type)
                }
                followPeersDB.Entry(userprofile).State = EntityState.Modified;
                followPeersDB.SaveChanges();
                if (userprofile.Contact == null) return RedirectToAction("EditContact", "Profile");

                return RedirectToAction("EditStudents", "Profile", new { message = "Successfully Updated" });
            }

            //   return View(userprofile);


        }

        private void UpdateStudent(string[] Name, string[] Type, string[] StartYear, string[] EndYear, string[] About, string[] Link, UserProfile userprofile)
        {
            int count = userprofile.Students.Count();
            for (int i = 0; i < count; i++)
            {
                Student tempStu = userprofile.Students.ElementAt(0);
                followPeersDB.Students.Remove(tempStu);
                //       userprofile.Educations.Remove(tempEdu);
            }
            if (Name != null)
            {
                for (int i = 0; i < Name.Count(); i++)
                {
                    if (Name != null)
                    {
                        string tempName = Name.ElementAt(i);
                        string tempStart = StartYear.ElementAt(i);
                        string tempEnd = EndYear.ElementAt(i);
                        string tempType = Type.ElementAt(i);
                        string tempAbout = About.ElementAt(i);
                        string tempUserId = Link.ElementAt(i);

                        Student tempStu = new Student
                        {
                            Name = tempName,
                            Type = Convert.ToInt16(tempType),
                            StartYear = Convert.ToInt16(tempStart),
                            EndYear = Convert.ToInt16(tempEnd),
                            About = tempAbout,
                            Link = Convert.ToInt16(tempUserId)
                        };
                        userprofile.Students.Add(tempStu);
                        tempStu.UserProfile = userprofile;
                    }
                }
            }
        }


        private void UpdateSpecializations(int[] Specialization, UserProfile userprofile)
        {
            if (Specialization != null)
            {
                var selectedSpecializations = new HashSet<int>(Specialization);
                var existingSpecializations = new HashSet<int>
                    (userprofile.Specializations.Select(c => c.SpecializationId));
                foreach (var s in followPeersDB.Specializations)
                {
                    //if new choices match with the database entries
                    if (selectedSpecializations.Contains(s.SpecializationId))
                    {
                        //if existing choices do not contain, add this new
                        if (!existingSpecializations.Contains(s.SpecializationId))
                        {
                            userprofile.Specializations.Add(s);
                            s.UserProfiles.Add(userprofile);
                        }
                    }
                    else
                    {
                        if (existingSpecializations.Contains(s.SpecializationId))
                        {
                            userprofile.Specializations.Remove(s);
                            Keyword temp = userprofile.Keywords.SingleOrDefault(p => p.Area == s.SpecializationName);
                            userprofile.Keywords.Remove(temp);
                            s.UserProfiles.Remove(userprofile);
                        }
                    }
                }

                int i = 0;
                foreach (var item in userprofile.Specializations)
                {
                    bool toadd = true;
                    if (userprofile.Keywords == null)
                    {
                        userprofile.Keywords = new List<Keyword>();
                    }
                    for (int j = 0; j < userprofile.Keywords.Count(); j++)
                    {
                        if (userprofile.Keywords.ElementAt(j).Area == userprofile.Specializations.ElementAt(i).SpecializationName)
                        { toadd = false; }
                    }
                    if (toadd == true)
                    {
                        userprofile.Keywords.Add(new Keyword { Area = userprofile.Specializations.ElementAt(i).SpecializationName });
                    }
                    i++;
                }


            }
        }

        [HttpPost]
        public ActionResult EditContact(FormCollection formCollection)
        {
            string name = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            bool toadd = false;
            if (userprofile.Contact == null) toadd = true;

            if (TryUpdateModel(userprofile, "", null, new string[] { "Country", "Educations", "Organization", "Specializations", "FirstName", "LastName", "Profession", "Gender", "Status" }))
            {
                userprofile.firsttime = false;
                if (userprofile.SignUpProgress + 0.2F <= 1.1F) userprofile.SignUpProgress += 0.20F;
                if (toadd == false) //this means the user is NOT editing the specialization records for the first time.. thus need to create an update record
                {
                    CreateUpdates("Contact information updated.", "/Profile/Index/" + userprofile.UserProfileId, 1, userprofile.UserProfileId); //CreateUpdates(message,link,type)
                }

                followPeersDB.Entry(userprofile).State = EntityState.Modified;
                followPeersDB.SaveChanges();

                return RedirectToAction("EditContact", "Profile", new { message = "Successfully Updated" });
            }
            ModelState.AddModelError("", "Last Name cannot be left blank.");
            return View(userprofile);

        }

        public ActionResult ChangeTier(string id, string tier)
        {
            string myname = Membership.GetUser().UserName;
            Relationship targetRelationship = followPeersDB.Relationships.SingleOrDefault(p => p.personAName == id && p.personBName == myname);
            int Tier = Convert.ToInt16(tier);
            targetRelationship.tier = Tier;
            followPeersDB.SaveChanges();
            return RedirectToAction("TierFollowers", "Profile", null);
        }
        public ActionResult DetailJob(int id)
        {
            Jobs j = followPeersDB.Jobs.First(p => p.JobId == id);
            string name = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            string temp = j.JobId.ToString();
            if (userprofile.Jobs.Contains(j) && j.ownerID != userprofile.UserProfileId)//need to add a button
                ViewData[temp] = "1";
            else
                ViewData[temp] = "0";

            return View(j);
        }

        [HttpGet]
        public ActionResult SearchJobs(string keywords, string country, string specialization, int sort)
        {
            ViewBag.sort = sort;
            string name = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            string Keywords = " ";
            Specialization spec = null;
            IEnumerable<Jobs> result = null;
            string searchstring = "Job Search Results ";
            if ((keywords != null) && (keywords != "") && (keywords.Length != 0))
            {
                Keywords = keywords;
                ViewData["keywords"] = keywords;
                searchstring += "For " + keywords;
            }
            if ((specialization != null) && (specialization != "Any"))
            {
                IEnumerable<Jobs> tempresult = null;
                spec = followPeersDB.Specializations.First(p => p.SpecializationName.Contains(specialization));
                tempresult = from j in followPeersDB.Jobs
                             where ((j.Title.Contains(Keywords) || (j.Description.Contains(Keywords))))
                             orderby j.Title
                             select j;
                searchstring += " " + specialization;
                if (country != null)
                {
                    tempresult = from j in followPeersDB.Jobs
                                 where ((j.Title.Contains(Keywords) || (j.Description.Contains(Keywords))) && (j.Country.Contains(country)))
                                 orderby j.Title
                                 select j;
                    searchstring = searchstring + " in " + country;
                }
                result = null;
                List<Jobs> temp = new List<Jobs>();
                foreach (var r in tempresult)
                {
                    /*if (r.Specializations.Contains(spec))
                    {
                        temp.Add(r);
                    }*/

                }
                foreach (var u in temp)
                {
                    string temp1 = u.JobId.ToString();
                    if (userprofile.Jobs.Contains(u) && u.ownerID != userprofile.UserProfileId)//need to add a button
                        ViewData[temp1] = "1";
                    else
                        ViewData[temp1] = "0";
                }
                if (searchstring == "Job Search Results ") searchstring = "Showing all jobs in the database";
                ViewBag.searchstring = searchstring;
                return View(temp);
            }

            else
            {
                result = from j in followPeersDB.Jobs
                         where ((j.Title.Contains(Keywords) || (j.Description.Contains(Keywords))))
                         orderby j.Title
                         select j;
                if (country != null)
                {
                    result = from j in followPeersDB.Jobs
                             where (j.Country.Contains(country) && (j.Title.Contains(Keywords) || (j.Description.Contains(Keywords))))
                             orderby j.Title
                             select j;
                    searchstring = searchstring + " in " + country;
                }
            }
            foreach (var u in result)
            {
                string temp = u.JobId.ToString();
                if (userprofile.Jobs.Contains(u) && u.ownerID != userprofile.UserProfileId)//need to add a button
                    ViewData[temp] = "1";
                else
                    ViewData[temp] = "0";
            }
            if (searchstring == "Job Search Results ") searchstring = "Showing all jobs in the database";
            ViewBag.searchstring = searchstring;
            return View(result);
        }
        [HttpGet]
        public string searchajax(string val)
        {
            string myname = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == myname);

            IEnumerable<UserProfile> people = from a in followPeersDB.UserProfiles
                                              where ((a.FirstName.Contains(val) || (a.LastName.Contains(val) || (a.UserName.Contains(val)) && a.activated == true)))
                                              orderby a.FirstName
                                              select a;

            IEnumerable<Jobs> jobs = from j in followPeersDB.Jobs
                                    where (j.Title.Contains(val) || j.Description.Contains(val))
                                    orderby j.Title
                                    select j;

            IEnumerable<PublicationModel> publication = from p in followPeersDB.PublicationModels
                                                        where (p.title.Contains(val) || p.description.Contains(val) || p.keyword.Contains(val))
                                                        orderby p.title
                                                        select p;

            IEnumerable<PatentModel> patent = from pa in followPeersDB.PatentModels
                                              where (pa.Title.Contains(val) || pa.Keyword.Contains(val) || pa.About.Contains(val))
                                              orderby pa.Title
                                              select pa;

            IEnumerable<Organization> organizations = from o in followPeersDB.Organizations
                                                      where o.Name.Contains(val)
                                                      orderby o.Name
                                                      select o;

            IEnumerable<Specialization> specializations = from s in followPeersDB.Specializations
                                                          where (s.SpecializationName.Contains(val) || s.Field.Contains(val))
                                                          orderby s.SpecializationName
                                                          select s;
            IEnumerable<ForumTopic> forumtopics = from t in followPeersDB.ForumTopics
                                                  where (t.Name.Contains(val) || t.Description.Contains(val) || t.Category.Contains(val))
                                                  orderby t.Name
                                                  select t;

            string returnstring = "";
            int totalresults = 0;
            if (people.Count() != 0)
            {
                int i = 0;
                returnstring += "<div class='each_rec' style='height: 9px;background: #777;'><a href='../../Profile/Search?keywords=" + val + "' style='height: 12px;line-height: 7px;width:100%;color: #ddd;'>People</a></div>";
                foreach (var p in people)
                {
                    if (i == 4) { break; }
                    string organization = p.Organization;
                    //if (p.FirstName.Length + p.LastName.Length + p.Organization.Length > 46) { organization = p.Organization.Substring(0, (46 - (p.FirstName.Length + p.LastName.Length))); organization += "..."; }
                    returnstring += "<div class='each_rec'><a href='../../Profile/Index/" + p.UserProfileId + "' style='margin-top: -4px;height: 28px;line-height: 25px;width:100%;'>" + p.FirstName + " " + p.LastName + " (" + organization + ")" + "</a></div>";
                    i++;
                    totalresults++;
                }
            }

            if (publication.Count() != 0)
            {
                int i = 0;
                returnstring += "<div class='each_rec' style='height: 9px;background: #777;'><a href='../../PublicationModel/Search?searchstring=" + val + "' style='height: 12px;line-height: 7px;width:100%;color: #ddd;'>Publication</a></div>";
                foreach (var p in publication)
                {
                    if (i == 4) { break; }
                    returnstring += "<div class='each_rec'><a href='../../PublicationModel/Details/" + p.publicationID + "' style='margin-top: -4px;height: 28px;line-height: 25px;width:100%;'>" + p.title + " " + p.type + "</a></div>";
                    i++;
                    totalresults++;
                }
            }

            if (patent.Count() != 0)
            {
                int i = 0;
                returnstring += "<div class='each_rec' style='height: 9px;background: #777;'><a href='../../PatentModel/Search?searchstring=" + val + "' style='height: 12px;line-height: 7px;width:100%;color: #ddd;'>Patent</a></div>";
                foreach (var p in patent)
                {
                    if (i == 4) { break; }
                    returnstring += "<div class='each_rec'><a href='../../PatentModel/Details/" + p.ID + "' style='margin-top: -4px;height: 28px;line-height: 25px;width:100%;'>" + p.Title + "</a></div>";
                    i++;
                    totalresults++;
                }
            }

            if (organizations.Count() != 0)
            {
                int i = 0;
                returnstring += "<div class='each_rec' style='height: 9px;background: #777;'><a href='../../Profile/Search?organization=" + val + "' style='height: 12px;line-height: 7px;width:100%;color: #ddd;'>Organizations</a></div>";
                foreach (var o in organizations)
                {
                    if (i == 2) { break; }
                    returnstring += "<div class='each_rec'><a href='../../Profile/Search?organization=" + o.Name + "' style='margin-top: -4px;height: 28px;line-height: 25px;width:100%;'>" + o.Name + "</a></div>";
                    i++;
                    totalresults++;
                }
            }

            if (specializations.Count() != 0)
            {
                int i = 0;
                returnstring += "<div class='each_rec' style='height: 9px;background: #777;'><a href='../../Profile/Search?specialization=" + val + "' style='height: 12px;line-height: 7px;width:100%;color: #ddd;'>Research Interests</a></div>";
                foreach (var s in specializations)
                {
                    if (i == 2) { break; }
                    returnstring += "<div class='each_rec'><a href='../../Profile/Search?specialization=" + s.SpecializationName + "' style='margin-top: -4px;height: 28px;line-height: 25px;width:100%;'>" + s.SpecializationName + "</a></div>";
                    i++;
                    totalresults++;
                }
            }

            if (jobs.Count() != 0)
            {
                int i = 0;
                returnstring += "<div class='each_rec' style='height: 9px;background: #777;'><a href='../../Profile/SearchJobs?sort=1&keywords=" + val + "' style='height: 12px;line-height: 7px;width:100%;color: #ddd;'>Jobs</a></div>";
                foreach (var j in jobs)
                {
                    if (i == 2) { break; }
                    returnstring += "<div class='each_rec'><a href='../../Profile/DetailJob/" + j.JobId + "' style='margin-top: -4px;height: 28px;line-height: 25px;width:100%;'>" + j.Title + "</a></div>";
                    i++;
                    totalresults++;
                }
            }

            if (forumtopics.Count() != 0)
            {
                int i = 0;
                returnstring += "<div class='each_rec' style='height: 9px;background: #777;'><a href='../../Forum/Search?q=" + val + "' style='height: 12px;line-height: 7px;width:100%;color: #ddd;'>Forum Topics</a></div>";
                foreach (var t in forumtopics)
                {
                    if (i == 2) { break; }
                    returnstring += "<div class='each_rec'><a href='../../Forum/TopicDetail/?Id=" + t.ForumTopicId + "' style='margin-top: -4px;height: 28px;line-height: 25px;width:100%;'>" + t.Category + "</a></div>";
                    i++;
                    totalresults++;
                }
            }

            //if (totalresults == 0) { returnstring += "<div class='each_rec'><a href='#' style='height: 18px;line-height: 15px;'>No results found</as></div>"; }
            returnstring += "<div class='each_rec2'><a href='../../Profile/Search?keywords=" + val + "' style='margin-top: -4px;height: 28px;line-height: 25px;width:100%;'>View More Results (" + val + ")</a></div>";
            return returnstring;
        }
        public string MarkNotificationsasSeen()
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);

            foreach (var noti in myprofile.Notifications)
            {
                noti.New = false;
            }
            followPeersDB.SaveChanges();
            return "";
        }
        public ActionResult ViewMoreNotifications()
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            var result = myprofile.Notifications.OrderByDescending(x => x.NotificationID);
            return View(result);
        }
        public string GetNotifications()
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            string returnstring = "";
            int i = 0;
            var notificationslist = myprofile.Notifications.OrderByDescending(x => x.NotificationID);
            foreach (var noti in notificationslist)
            {
                if (i == 5) break;
                string msg = noti.message;
                if (noti.message.Length > 75) msg = noti.message.Substring(0, 70) + "...";
                returnstring += "<div class='each_rec'><div style='height: 50px;overflow: hidden;float: left;'><img src='" + noti.imagelink + "' style='width:50px;'></div><div style='float: left;width: 70%;text-align:left;'><a href='" + noti.link + "' style='height: 50px;line-height: 14px;width: 100%;font-weight: normal;'>" + msg + "</a></div><div style='clear: both;'></div></div>";
                i++;
            }

            if (myprofile.Notifications.Count() == 0)
            {
                returnstring += "<div class='each_rec'><div style='float: left;width: 95%;'><a href='#' style='height: 25px;line-height: 14px;width: 100%;font-weight: normal;'>You have no notifications.</a></div><div style='clear: both;'></div></div>";
            }
            if (notificationslist.Count() != 0) returnstring += "<div class='each_rec'><div style='float: left;width: 95%;'><a href='../../Profile/ViewMoreNotifications' style='height: 25px;line-height: 14px;width: 100%;font-weight: normal;'> View More Notifications </a></div><div style='clear: both;'></div></div>";
            return returnstring;
        }
        public string GetNumberofNewNoti()
        {
            int count = 0;
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            foreach (var noti in myprofile.Notifications)
            {
                if (noti.New == true) { count++; }
            }
            if (count == 0) return "";
            return count.ToString();
        }
        public ActionResult ViewBookmarkJobs()
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            List<Jobs> result = myprofile.Jobs.ToList();
            List<Jobs> tempresult = myprofile.Jobs.ToList();
            foreach (var j in tempresult)
            {
                if (j.ownerID == myprofile.UserProfileId) { result.Remove(j); }
            }
            return View(result);
        }

        public ActionResult ViewBookmarkPublications()
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            IEnumerable<Bookmark> result = from b in followPeersDB.Bookmarks
                                           where (b.userID == myprofile.UserProfileId && b.bookmarkType == "Publication")
                                           select b;
            List<PublicationModel> tempresult = new List<PublicationModel>();
            foreach (var b in result)
            {
                PublicationModel temppublication = followPeersDB.PublicationModels.SingleOrDefault(p => p.publicationID == b.itemID);
                tempresult.Add(temppublication);
            }

            return View(tempresult);
        }

        public ActionResult ViewBookmarkPatents()
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            IEnumerable<Bookmark> result = from b in followPeersDB.Bookmarks
                                           where (b.userID == myprofile.UserProfileId && b.bookmarkType == "Patent")
                                           select b;
            List<PatentModel> tempresult = new List<PatentModel>();
            foreach (var b in result)
            {
                PatentModel temppublication = followPeersDB.PatentModels.SingleOrDefault(p => p.ID == b.itemID);
                tempresult.Add(temppublication);
            }

            return View(tempresult);
        }

        [HttpGet]
        public ActionResult Search(string keywords, string department, string organization, string specialization)
        {
            string myname = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == myname);

            if (organization != null)
            {
                if (department == null)
                {
                    IEnumerable<UserProfile> result = from UserProfile in followPeersDB.UserProfiles
                                                      where (UserProfile.Organization.Contains(organization) && UserProfile.activated == true)
                                                      orderby UserProfile.UserName
                                                      select UserProfile;
                    ViewBag.searchstring = "People from " + organization;
                    foreach (var u in result)
                    {
                        string temp = u.UserName;
                        if ((followPeersDB.Relationships.SingleOrDefault(p => p.personAName == userprofile.UserName && p.personBName == u.UserName) != null))//already followed
                            ViewData[temp] = "1";
                        else
                            ViewData[temp] = "0";
                    }

                    return View(result);
                }
                else
                {
                    IEnumerable<UserProfile> result = from UserProfile in followPeersDB.UserProfiles
                                                      where (UserProfile.Organization.Contains(organization) && UserProfile.Departments.Contains(department) && UserProfile.activated == true)
                                                      orderby UserProfile.UserName
                                                      select UserProfile;
                    ViewBag.searchstring = "People from " + department + ", " + organization;
                    foreach (var u in result)
                    {
                        string temp = u.UserName;
                        if ((followPeersDB.Relationships.SingleOrDefault(p => p.personAName == userprofile.UserName && p.personBName == u.UserName) != null))//already followed
                            ViewData[temp] = "1";
                        else
                            ViewData[temp] = "0";
                    }

                    return View(result);
                }

            }

            else if (department != null)
            {
                IEnumerable<UserProfile> result = from UserProfile in followPeersDB.UserProfiles
                                                  where (UserProfile.Departments.Contains(department) && UserProfile.activated == true)
                                                  orderby UserProfile.UserName
                                                  select UserProfile;
                ViewBag.searchstring = "People from " + department;
                foreach (var u in result)
                {
                    string temp = u.UserName;
                    if ((followPeersDB.Relationships.SingleOrDefault(p => p.personAName == userprofile.UserName && p.personBName == u.UserName) != null))//already followed
                        ViewData[temp] = "1";
                    else
                        ViewData[temp] = "0";
                }

                return View(result);
            }


            else if (keywords != null && keywords.Length != 0)
            {
                ViewData["keywords"] = keywords;
                IEnumerable<UserProfile> result = from UserProfile in followPeersDB.UserProfiles
                                                  where (UserProfile.UserName.Contains(keywords) || UserProfile.FirstName.Contains(keywords) || UserProfile.LastName.Contains(keywords)) && UserProfile.activated == true
                                                  orderby UserProfile.UserName
                                                  select UserProfile;
                ViewBag.searchstring = "People Search Results for " + keywords;
                foreach (var u in result)
                {
                    string temp = u.UserName;
                    if ((followPeersDB.Relationships.SingleOrDefault(p => p.personAName == userprofile.UserName && p.personBName == u.UserName) != null))//already followed
                        ViewData[temp] = "1";
                    else
                        ViewData[temp] = "0";
                }

                return View(result);
            }

            else if (specialization != null)
            {
                IEnumerable<UserProfile> result = from UserProfile in followPeersDB.UserProfiles
                                                  where (UserProfile.Specializations.Any(i => i.SpecializationName.Contains(specialization)) && UserProfile.activated == true)
                                                  orderby UserProfile.UserName
                                                  select UserProfile;
                ViewBag.searchstring = "People with research interest in " + specialization;
                foreach (var u in result)
                {
                    string temp = u.UserName;
                    if ((followPeersDB.Relationships.SingleOrDefault(p => p.personAName == userprofile.UserName && p.personBName == u.UserName) != null))//already followed
                        ViewData[temp] = "1";
                    else
                        ViewData[temp] = "0";
                }

                return View(result);
            }
            else
            {
                IEnumerable<UserProfile> result = from UserProfile in followPeersDB.UserProfiles
                                                  orderby UserProfile.UserName
                                                  select UserProfile;
                ViewBag.searchstring = "Showing all people within the network";
                foreach (var u in result)
                {
                    string temp = u.UserName;
                    if ((followPeersDB.Relationships.SingleOrDefault(p => p.personAName == userprofile.UserName && p.personBName == u.UserName) != null))//already followed
                        ViewData[temp] = "1";
                    else
                        ViewData[temp] = "0";
                }
                return View(result);
            }

        }
        [HttpPost]
        public void Follow(string username, string url)
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            // UserProfile followerProfile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == username);
            // UserProfile followerProfile = new UserProfile();

            Relationship tempR = new Relationship
            {
                personAName = name,
                personBName = username,
                tier = myprofile.Default //setting the default tier
            };

            followPeersDB.Relationships.Add(tempR);
            followPeersDB.SaveChanges();

            UserProfile personB = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == username);
            UserProfile follower = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            Notification newnoti = new Notification
            {
                message = follower.FirstName + " started following you.",
                link = "/Profile/Index/" + follower.UserProfileId,
                New = true,
                imagelink = myprofile.PhotoUrl,
            };

            personB.Notifications.Add(newnoti);
            followPeersDB.Entry(personB).State = EntityState.Modified;
            followPeersDB.SaveChanges();


            Response.Redirect(url);
            // return RedirectToAction("Index", "Profile", new { id = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == username).UserProfileId });

        }


        [HttpPost]
        public void Bookmark(string jobid, string url)
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            int jobidINT = Convert.ToInt16(jobid);
            // UserProfile followerProfile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == username);
            // UserProfile followerProfile = new UserProfile();
            Jobs job = followPeersDB.Jobs.SingleOrDefault(p => p.JobId == jobidINT);
            myprofile.Jobs.Add(job);
            followPeersDB.SaveChanges();

            UserProfile updateowner = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserProfileId == job.ownerID);
            string jobdescription = job.Description;
            if (jobdescription.Length > 60) jobdescription = job.Description.Substring(0, 60) + "...";
            Notification newnoti = new Notification
            {
                message = myprofile.FirstName + " bookmarked your job post : " + jobdescription,
                link = "/Profile/Index/" + myprofile.UserProfileId,
                New = true,
                imagelink = myprofile.PhotoUrl,
            };

            updateowner.Notifications.Add(newnoti);
            followPeersDB.Entry(updateowner).State = EntityState.Modified;
            followPeersDB.SaveChanges();

            Response.Redirect(url);
            // return RedirectToAction("Index", "Profile", new { id = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == username).UserProfileId });

        }

        [HttpPost]
        public void UnBookmark(string jobid, string url)
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            int jobidINT = Convert.ToInt16(jobid);
            // UserProfile followerProfile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == username);
            // UserProfile followerProfile = new UserProfile();
            Jobs job = followPeersDB.Jobs.SingleOrDefault(p => p.JobId == jobidINT);
            myprofile.Jobs.Remove(job);
            followPeersDB.SaveChanges();
            Response.Redirect(url);
            // return RedirectToAction("Index", "Profile", new { id = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == username).UserProfileId });

        }
        public ActionResult CreateGroup(string name, string[] members)
        {
            string myname = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == myname);
            Group grp = new Group
            {
                Name = name,
                OwnerId = myprofile.UserProfileId,
                Members = new List<UserProfile>()
            };
            if (members == null || members.Count() == 0) return RedirectToAction("Index", "Bulletin", new { sort = 3 });
            for (int i = 0; i < members.Count(); i++)
            {
                string tempstr = members[i];

                IQueryable<UserProfile> custQuery = from cust in followPeersDB.UserProfiles where cust.FirstName + " " + cust.LastName == tempstr select cust;
                foreach (var profile in custQuery)
                {
                    grp.Members.Add(profile);
                }

            }
            followPeersDB.Groups.Add(grp);
            followPeersDB.SaveChanges();
            return RedirectToAction("Index", "Bulletin", new { sort = 3 });

        }
        public ActionResult RemoveFromGrp(string name, string grpname)
        {
            int id = Convert.ToInt16(name);
            string myname = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == myname);
            UserProfile profile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserProfileId == id);

            Group grp = followPeersDB.Groups.SingleOrDefault(p => p.Name == grpname & p.OwnerId == myprofile.UserProfileId); //come back here

            grp.Members.Remove(profile);
            if (grp.Members.Count() == 0)
                followPeersDB.Groups.Remove(grp);
            //profile.Groups.Remove(grp);
            followPeersDB.SaveChanges();
            return RedirectToAction("Index", "Bulletin", new { sort = 3 });

        }

        public ActionResult AddMembertoGroup(string name, string grpname)
        {
            string myname = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == myname);
            UserProfile profile = followPeersDB.UserProfiles.SingleOrDefault(p => p.FirstName + " " + p.LastName == name);

            IQueryable<Group> custQuery = from cust in followPeersDB.Groups where cust.OwnerId == myprofile.UserProfileId select cust;
            foreach (var grp in custQuery)
            {
                if (grp.Members.Contains(profile))
                {
                    if (grp.Name == grpname) { return RedirectToAction("Index", "Bulletin", new { sort = 3 }); }
                    grp.Members.Remove(profile);
                }
                if (grp.Members.Count() == 0)
                    followPeersDB.Groups.Remove(grp);
            }

            Group Togrp = followPeersDB.Groups.SingleOrDefault(p => p.OwnerId == myprofile.UserProfileId & p.Name == grpname); //come back here

            Togrp.Members.Add(profile);

            followPeersDB.SaveChanges();
            return RedirectToAction("Index", "Bulletin", new { sort = 3 });

        }

        public ActionResult DeleteGroup(string name)
        {
            string myname = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == myname);
            Group grp = followPeersDB.Groups.SingleOrDefault(p => p.Name == name & p.OwnerId == myprofile.UserProfileId);

            followPeersDB.Groups.Remove(grp);
            followPeersDB.SaveChanges();
            return RedirectToAction("Index", "Bulletin", new { sort = 3 });

        }
        [HttpPost]
        public void UnFollow(string username, string url)
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            // UserProfile followerProfile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == username);
            // UserProfile followerProfile = new UserProfile();

            //  Relationship tempR = followPeersDB.Relationships.SingleOrDefault(p=> p.personAName == name && p.personBName==username);

            followPeersDB.Relationships.Remove(followPeersDB.Relationships.SingleOrDefault(p => p.personAName == name && p.personBName == username));
            followPeersDB.SaveChanges();

            UserProfile personB = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == username);
            UserProfile follower = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            Notification newnoti = new Notification
            {
                message = follower.FirstName + " stopped following you.",
                link = "/Profile/Index/" + follower.UserProfileId,
                New = true,
                imagelink = myprofile.PhotoUrl,
            };

            personB.Notifications.Add(newnoti);
            followPeersDB.Entry(personB).State = EntityState.Modified;
            followPeersDB.SaveChanges();


            Response.Redirect(url);
            //return RedirectToAction("Index", "Profile", new { id = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == username).UserProfileId });

        }

        public ActionResult Info()
        {
            return PartialView();
        }

        public ActionResult NoticeBoard()
        {
            return PartialView();
        }

        public ActionResult Publications()
        {
            return PartialView();
        }

        public ActionResult Patents()
        {
            return PartialView();
        }

        //
        // GET: /Profile/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }



        //
        // GET: /Profile/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Profile/Delete/5

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
        internal List<Organization> FindOrganization(string searchText, int maxResults)
        {
            var result = from n in followPeersDB.Organizations
                         where n.Name.Contains(searchText)
                         orderby n.Name
                         select n;

            return result.Take(maxResults).ToList();

        }
        [HttpPost]
        public JsonResult FindOrganizationNames(string searchText, int maxResults)
        {
            var result = FindOrganization(searchText, maxResults);
            return Json(result);
        }

        internal List<Department> FindDepartment(string searchText, int maxResults)
        {
            var result = from n in followPeersDB.Departments
                         where n.Name.Contains(searchText)
                         orderby n.Name
                         select n;

            return result.Take(maxResults).ToList();

        }
        [HttpPost]
        public JsonResult FindDepartmentNames(string searchText, int maxResults)
        {
            var result = FindDepartment(searchText, maxResults);
            return Json(result);
        }

        [HttpPost]
        public JsonResult FindUserNames(string searchText, int maxResults)
        {
            var result = from n in followPeersDB.UserProfiles
                         where ((n.FirstName.Contains(searchText)) || (n.LastName.Contains(searchText)) || ((n.FirstName + " " + n.LastName).Contains(searchText)))
                         orderby n.FirstName
                         select new
                         {
                             FirstName = n.FirstName,
                             LastName = n.LastName,
                             UserProfileId = n.UserProfileId,
                             PhotoUrl = n.PhotoUrl,
                             Organization = n.Organization,
                             UserName = n.UserName
                         };
            return Json(result);

        }


        public JsonResult FindSpecializations(string q)
        {
            var result = from n in followPeersDB.Specializations
                         where n.SpecializationName.Contains(q)
                         orderby n.SpecializationName
                         select new
                         {
                             value = n.SpecializationId,
                             name = n.SpecializationName,

                         };
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult FindFriends(string q)
        {
            var result = from n in followPeersDB.UserProfiles
                         where ((n.FirstName.Contains(q)) || (n.LastName.Contains(q)) || ((n.FirstName + " " + n.LastName).Contains(q)))
                         orderby n.FirstName
                         select new
                         {
                             value = n.UserProfileId,
                             name = n.FirstName + " " + n.LastName,

                         };
            return Json(result, JsonRequestBehavior.AllowGet);

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
        public ActionResult TierFollowers()
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);

            return View(myprofile);
        }
        public ActionResult Privacy()
        {
            string name = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);

            return View(myprofile);
        }

        [HttpPost]
        public ActionResult Privacy(int? Email1, int? Email2, int? Email3, int? Phone1, int? Phone2, int? Phone3, int? Mobile1, int? Mobile2, int? Mobile3, int? Address1, int? Address2, int? Address3, int? Education1, int? Education2, int? Education3, int? Publication1, int? Publication2, int? Publication3, int? Patent1, int? Patent2, int? Patent3, int? Noticeboard1, int? Noticeboard2, int? Noticeboard3, int? Aboutme1, int? Aboutme2, int? Aboutme3, int Default)
        {
            string name = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            if (TryUpdateModel(userprofile, "", null, new string[] { "Specializations" }))
            {
                if (Email1 != null) userprofile.Tiers.ElementAt(0).Email = 1;
                else userprofile.Tiers.ElementAt(0).Email = 0;
                if (Email2 != null) userprofile.Tiers.ElementAt(1).Email = 1;
                else userprofile.Tiers.ElementAt(1).Email = 0;
                if (Email3 != null) userprofile.Tiers.ElementAt(2).Email = 1;
                else userprofile.Tiers.ElementAt(2).Email = 0;

                if (Phone1 != null) userprofile.Tiers.ElementAt(0).Phone = 1;
                else userprofile.Tiers.ElementAt(0).Phone = 0;
                if (Phone2 != null) userprofile.Tiers.ElementAt(1).Phone = 1;
                else userprofile.Tiers.ElementAt(1).Phone = 0;
                if (Phone3 != null) userprofile.Tiers.ElementAt(2).Phone = 1;
                else userprofile.Tiers.ElementAt(2).Phone = 0;

                if (Mobile1 != null) userprofile.Tiers.ElementAt(0).Mobile = 1;
                else userprofile.Tiers.ElementAt(0).Mobile = 0;
                if (Mobile2 != null) userprofile.Tiers.ElementAt(1).Mobile = 1;
                else userprofile.Tiers.ElementAt(1).Mobile = 0;
                if (Mobile3 != null) userprofile.Tiers.ElementAt(2).Mobile = 1;
                else userprofile.Tiers.ElementAt(2).Mobile = 0;

                if (Address1 != null) userprofile.Tiers.ElementAt(0).Address = 1;
                else userprofile.Tiers.ElementAt(0).Address = 0;
                if (Address2 != null) userprofile.Tiers.ElementAt(1).Address = 1;
                else userprofile.Tiers.ElementAt(1).Address = 0;
                if (Address3 != null) userprofile.Tiers.ElementAt(2).Address = 1;
                else userprofile.Tiers.ElementAt(2).Address = 0;

                if (Education1 != null) userprofile.Tiers.ElementAt(0).Education = 1;
                else userprofile.Tiers.ElementAt(0).Education = 0;
                if (Education2 != null) userprofile.Tiers.ElementAt(1).Education = 1;
                else userprofile.Tiers.ElementAt(1).Education = 0;
                if (Education3 != null) userprofile.Tiers.ElementAt(2).Education = 1;
                else userprofile.Tiers.ElementAt(2).Education = 0;

                if (Publication1 != null) userprofile.Tiers.ElementAt(0).Publication = 1;
                else userprofile.Tiers.ElementAt(0).Publication = 0;
                if (Publication2 != null) userprofile.Tiers.ElementAt(1).Publication = 1;
                else userprofile.Tiers.ElementAt(1).Publication = 0;
                if (Publication3 != null) userprofile.Tiers.ElementAt(2).Publication = 1;
                else userprofile.Tiers.ElementAt(2).Publication = 0;

                if (Patent1 != null) userprofile.Tiers.ElementAt(0).Patent = 1;
                else userprofile.Tiers.ElementAt(0).Patent = 0;
                if (Patent2 != null) userprofile.Tiers.ElementAt(1).Patent = 1;
                else userprofile.Tiers.ElementAt(1).Patent = 0;
                if (Patent3 != null) userprofile.Tiers.ElementAt(2).Patent = 1;
                else userprofile.Tiers.ElementAt(2).Patent = 0;

                if (Noticeboard1 != null) userprofile.Tiers.ElementAt(0).Noticeboard = 1;
                else userprofile.Tiers.ElementAt(0).Noticeboard = 0;
                if (Noticeboard2 != null) userprofile.Tiers.ElementAt(1).Noticeboard = 1;
                else userprofile.Tiers.ElementAt(1).Noticeboard = 0;
                if (Noticeboard3 != null) userprofile.Tiers.ElementAt(2).Noticeboard = 1;
                else userprofile.Tiers.ElementAt(2).Noticeboard = 0;

                if (Aboutme1 != null) userprofile.Tiers.ElementAt(0).AboutMe = 1;
                else userprofile.Tiers.ElementAt(0).AboutMe = 0;
                if (Aboutme2 != null) userprofile.Tiers.ElementAt(1).AboutMe = 1;
                else userprofile.Tiers.ElementAt(1).AboutMe = 0;
                if (Aboutme3 != null) userprofile.Tiers.ElementAt(2).AboutMe = 1;
                else userprofile.Tiers.ElementAt(2).AboutMe = 0;

                userprofile.Default = Default;

                followPeersDB.Entry(userprofile).State = EntityState.Modified;
                followPeersDB.SaveChanges();
                return RedirectToAction("Index", "Bulletin", new { sort = 3 });
            }

            if (userprofile.FirstName == null) ModelState.AddModelError("", "First Name cannot be left blank.");
            if (userprofile.LastName == null) ModelState.AddModelError("", "Last Name cannot be left blank.");

            return View(userprofile);
        }
        [HttpPost]
        public ActionResult UpdateStatus(string message)
        {
            string name = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            userprofile.StatusMessage = message;
            CreateUpdates(message, "/Notice/Index/" + userprofile.UserProfileId, 2, userprofile.UserProfileId);
            followPeersDB.Entry(userprofile).State = EntityState.Modified;
            followPeersDB.SaveChanges();
            return RedirectToAction("Index", "Notice", new { id = userprofile.UserProfileId });
        }
        [HttpPost]
        public ActionResult PosttoNoticeBoard(string message, string redirect)
        {
            string name = Membership.GetUser().UserName;
            UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            // userprofile.StatusMessage = message;
            int id = Convert.ToInt16(redirect);
            CreateUpdates(message, "/Notice/Index/" + id, 5, id);
            followPeersDB.Entry(userprofile).State = EntityState.Modified;
            followPeersDB.SaveChanges();

            UserProfile Noticeowner = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserProfileId == id);
            if (message.Length > 30) message = message.Substring(0, 25) + "...";

            Notification newnoti = new Notification
            {
                message = userprofile.FirstName + " wrote on your Noticeboard : " + message,
                link = "/Notice/Index/" + id,
                New = true,
                imagelink = userprofile.PhotoUrl,
            };
            Noticeowner.Notifications.Add(newnoti);
            followPeersDB.Entry(Noticeowner).State = EntityState.Modified;
            followPeersDB.SaveChanges();


            return RedirectToAction("Index", "Notice", new { id = id });
        }

        [HttpPost]
        public ActionResult AddComment(int commentid, string message, int redirect)
        {
            string name = Membership.GetUser().UserName;
            UserProfile myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);

            Update update = followPeersDB.Updates.SingleOrDefault(p => p.UpdateId == commentid);
            // UserProfile user = update.UserProfiles.ElementAt(0);
            //if (myprofile.UserProfileId != update.owner)
            if (myprofile.UserProfileId != update.UserProfiles.ElementAt(0).UserProfileId)
            {
                CreateUpdates(message, "/Notice/Index/" + myprofile.UserProfileId, 3, myprofile.UserProfileId);
                followPeersDB.Entry(myprofile).State = EntityState.Modified;
            }

            NoticeComment comment = new NoticeComment() { commenter = myprofile, message = message, time = DateTime.Now, update = update };
            followPeersDB.NoticeComments.Add(comment);

            followPeersDB.SaveChanges();

            //when adding a comment on a noticeboard, need to create a notification item also

            //notification items have to be created for 
            // 1. owner of the update the comment is attached to (if owner is not the same as current commenter)
            // 2. commenters of other comments under the same update (if they are not the same as current commenter)
            // 3. owner of the noticeboard (if owner is not the same as current commenter)

            List<UserProfile> notiaddedlist = new List<UserProfile>();
            //Case 1 owner of the update
            if (update.owner != myprofile.UserProfileId)
            {
                UserProfile updateowner = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserProfileId == update.owner);
                UserProfile user = update.UserProfiles.ElementAt(0);
                Notification newnoti = new Notification
                {
                    message = myprofile.FirstName + " commented on your post : " + message,
                    link = "/Notice/Index/" + user.UserProfileId,
                    New = true,
                    imagelink = myprofile.PhotoUrl,
                };
                var toadd = true;
                //to prevent adding notifications to the same person twice
                foreach (var item in notiaddedlist)
                {
                    if (item == updateowner) toadd = false;
                }
                if (toadd == true)
                {
                    notiaddedlist.Add(updateowner);
                    updateowner.Notifications.Add(newnoti);
                    followPeersDB.Entry(updateowner).State = EntityState.Modified;
                    followPeersDB.SaveChanges();
                }
            }

            //Case 2. commenters of other comments under the same update (if they are not the same as current commenter OR the noticeboard owner)
            IEnumerable<NoticeComment> result = from n in followPeersDB.NoticeComments
                                                where n.update.UpdateId == update.UpdateId
                                                select n;
            int noticeownerid = update.UserProfiles.ElementAt(0).UserProfileId;
            foreach (var n in result)
            {
                if ((n.commenterId != myprofile.UserProfileId) && (n.commenterId != noticeownerid))
                {
                    UserProfile commenter = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserProfileId == n.commenterId);
                    UserProfile user = update.UserProfiles.ElementAt(0);
                    Notification newnoti = new Notification
                    {
                        message = myprofile.FirstName + " commented on your post : " + message,
                        link = "/Notice/Index/" + user.UserProfileId,
                        New = true,
                        imagelink = myprofile.PhotoUrl,
                    };
                    var toadd = true;
                    //to prevent adding notifications to the same person twice
                    foreach (var item in notiaddedlist)
                    {
                        if (item == commenter) toadd = false;
                    }
                    if (toadd == true)
                    {
                        notiaddedlist.Add(commenter);
                        commenter.Notifications.Add(newnoti);
                        followPeersDB.Entry(commenter).State = EntityState.Modified;
                        followPeersDB.SaveChanges();
                    }
                }
            }
            //Case 3. owner of the noticeboard (if owner is not the same as current commenter)
            if (noticeownerid != myprofile.UserProfileId)
            {

                UserProfile noticeowner = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserProfileId == noticeownerid);
                Notification newnoti = new Notification
                {
                    message = myprofile.FirstName + " commented on your Noticeboard : " + message,
                    link = "/Notice/Index/" + noticeownerid,
                    New = true,
                    imagelink = myprofile.PhotoUrl,
                };
                var toadd = true;
                //to prevent adding notifications to the same person twice
                foreach (var item in notiaddedlist)
                {
                    if (item == noticeowner) toadd = false;
                }
                if (toadd == true)
                {
                    notiaddedlist.Add(noticeowner);
                    noticeowner.Notifications.Add(newnoti);
                    followPeersDB.Entry(noticeowner).State = EntityState.Modified;
                    followPeersDB.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Notice", new { id = redirect });
        }


        public ActionResult UpdateKeywords(string keywordlist)
        {
            string myname = Membership.GetUser().UserName;
            myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == myname);

            int count = myprofile.Keywords.Count();
            for (int i = 0; i < count; i++)
            {
                Keyword temp = myprofile.Keywords.ElementAt(0);
                myprofile.Keywords.Remove(temp);
                followPeersDB.Keywords.Remove(temp);

            }
            char[] delimiterChars = { ',' };
            string[] words = keywordlist.Split(delimiterChars);

            foreach (string s in words)
            {
                string s2 = s.Trim();
                Keyword temp = new Keyword { Area = s2 };
                myprofile.Keywords.Add(temp);
            }
            followPeersDB.Entry(myprofile).State = EntityState.Modified;
            followPeersDB.SaveChanges();
            return RedirectToAction("Index", "News", new { reader = "Yahoo" });
        }

    }
}