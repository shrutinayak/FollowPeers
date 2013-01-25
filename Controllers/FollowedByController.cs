using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FollowPeers.Models;
using System.Data;
using System.Data.Entity;

namespace FollowPeers.Controllers
{
    public class FollowedByController : Controller
    {
        //
        // GET: /FollowedBy/
        FollowPeersDBEntities followPeersDB = new FollowPeersDBEntities();
        public ActionResult Index(int sort)
        {
            string checkname = Session["username"].ToString();
            List<UserProfile> result = new List<UserProfile>();
            string myname = Membership.GetUser().UserName;
            IQueryable<string> custQuery = from cust in followPeersDB.Relationships where cust.personBName == checkname select cust.personAName;
            IQueryable<string> custQuery2 = from cust in followPeersDB.Relationships where cust.personAName == checkname select cust.personBName;
            List<string> duplicates = new List<string>();
            int followcount = 0;
            int followerscount = 0;
            foreach (string personAname in custQuery2)
            {
                result.Add(followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == personAname));
                ViewData[personAname] = "1";
                followcount += 1;
            }
            foreach (string personBname in custQuery)
            {
                result.Add(followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == personBname));
                ViewData[personBname] = "0";
                followerscount += 1;
            }
            foreach (string person1 in custQuery)
            {
                foreach (string person2 in custQuery2)
                {
                    if (person1 == person2)
                    {
                        ViewData[person1] = "2";
                        duplicates.Add(person1);
                    }
                }
            }
            ViewData["followscount"] = followcount;
            ViewData["followerscount"] = followerscount;
            foreach (string name in duplicates)
            {
                result.Remove(followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name));
            }
            /* foreach (var u in result)
             {
                 string temp = u.UserName;
                 if ((followPeersDB.Relationships.SingleOrDefault(p => p.personAName == myname && p.personBName == u.UserName) != null))//already followed
                     ViewData[temp] = "1";
                 else if((followPeersDB.Relationships.SingleOrDefault(p => p.personBName == myname && p.personAName == u.UserName) != null))
                     ViewData[temp] = "2";
                 else
                     ViewData[temp] = "0";
             }*/
            ViewData["link"] = checkname;
            if (checkname == myname)
            { ViewData["myself"] = 1; }
            else
            { ViewData["myself"] = 0; }
            ViewBag.sort = sort;
            return View(result);
        }

        public ActionResult ViewPage1()
        {
            return View();
        }

        public ActionResult MarkCommentAsRead(int id)
        {
            string name = Membership.GetUser().UserName;
            UserProfile myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
            foreach (var item in myprofile.Updates)
            {
                if (item.owner == id)
                {
                    item.New = false; //set the update as viewed
                    followPeersDB.Entry(item).State = EntityState.Modified;
                    followPeersDB.SaveChanges();
                }
            }
            return RedirectToAction("Index", "FollowedBy", new { sort = 3 });
        }
    }
}
