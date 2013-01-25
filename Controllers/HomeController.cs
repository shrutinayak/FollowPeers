using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FollowPeers.Models;

namespace FollowPeers.Controllers
{
    public class HomeController : Controller
    {
        FollowPeersDBEntities followPeersDB = new FollowPeersDBEntities();
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                //return View();
                string name = Membership.GetUser().UserName;
                UserProfile myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
                if (myprofile == null) //if the database stores a wrong info about name
                {
                    name = " " + name;
                    myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
                    myprofile.UserName.Trim();
                    followPeersDB.SaveChanges();

                }
                ViewBag.username = name;
                    return RedirectToAction("Index", "Bulletin", new { sort = 3 });
            }
            else // load the home page
            {
                return View();
            }
        }

        public ActionResult About()
        {
            return View();
        }

    }
}
