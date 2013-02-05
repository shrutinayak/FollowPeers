using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FollowPeers.Models;
using System.Data;
using System.Data.Entity;
using System.Web.Security;

namespace FollowPeers.Controllers
{
    public class EventController : Controller
    {
        FollowPeersDBEntities db = new FollowPeersDBEntities();
        string name = Membership.GetUser().UserName;
        static UserProfile myprofile;
        //
        // GET: /Event/

        public ActionResult Index()
        {
            myprofile = db.UserProfiles.SingleOrDefault(p => p.UserName == name);
            //return View();
            var result = from n in db.Events
                         orderby n.EventDateTime ascending
                         select n;
            return View(result.ToList());

        }


        //
        // GET: /Event/AddEvent

        public ActionResult AddEvent()
        {
            return View();
        }

        //
        // POST: /Event/AddEvent
        [HttpPost]
        public ActionResult AddEvent(Event model)
        {
            if (ModelState.IsValid)
            {
                if (model.EventName == null)
                {
                    model.EventName = "default";
                }
                if (model.EventDateTime == DateTime.MinValue)
                {
                    model.EventDateTime = DateTime.Now;
                }
                if (model.EventDesc == null)
                {
                    model.EventDesc = "default";
                }

                string name = Membership.GetUser().UserName;
                UserProfile user = db.UserProfiles.SingleOrDefault(p => p.UserName == name);
                //model.UserProfile = user;
                //user.Publication.Add(publicationmodel);
                
                try
                {
                    model.EventId = db.Events.Count() + 1;
                }
                catch
                {//If Events has no items yet ie count() returns an error
                    model.EventId = 1;
                }
                //Add the model to the table
                db.Events.Add(model);

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            else
            {  //Something went wrong in creating the event
                return RedirectToAction("AddEvent");
            }
        }
    }
}
