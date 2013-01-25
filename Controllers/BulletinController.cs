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
    public class BulletinController : Controller
    {
        //
        // GET: /Notice/
        FollowPeersDBEntities followPeersDB = new FollowPeersDBEntities();
        public ActionResult Index(int sort)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string name = Membership.GetUser().UserName;
                UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
                ViewBag.sort = sort;

                 return View(userprofile);
            }
            else
                return RedirectToAction("Index", "Home", null);
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
            return RedirectToAction("Index", "Bulletin", new {sort = 3});
        }

    }
}
