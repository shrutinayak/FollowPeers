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
    public class NoticeController : Controller
    {
        //
        // GET: /Notice/
        FollowPeersDBEntities followPeersDB = new FollowPeersDBEntities();
        public ActionResult Index(int id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string name = Membership.GetUser().UserName;
                UserProfile myprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserName == name);
                UserProfile userprofile = followPeersDB.UserProfiles.SingleOrDefault(p => p.UserProfileId == id);

                return View(userprofile);
            }
            else
                return RedirectToAction("Index", "Home", null);
        }
        
        public ActionResult DeleteComment(int id,int redirect)
        {
            NoticeComment mycomment = followPeersDB.NoticeComments.SingleOrDefault(p => p.NoticeCommentId == id);
            string message = mycomment.message;
            followPeersDB.NoticeComments.Remove(mycomment);
            followPeersDB.Entry(mycomment).State = EntityState.Deleted;
            followPeersDB.SaveChanges();

            //Delete the update as well
            Update myupdate = followPeersDB.Updates.SingleOrDefault(p => p.message == message && p.Own == true);
            followPeersDB.Updates.Remove(myupdate);
            followPeersDB.Entry(myupdate).State = EntityState.Deleted;
            followPeersDB.SaveChanges();
            return RedirectToAction("Index", "Notice", new { id = redirect });
        }
       

    }
}
