using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FollowPeers.Models;
using System.Web.Security;

namespace FollowPeers.Controllers
{
    public class ForumController : Controller
    {
        FollowPeersDBEntities db = new FollowPeersDBEntities();
        //
        // GET: /Forum/

        public ActionResult Index(string Category)
        {
            Forum toReturn = new Forum();
            if (Category == null || Category == "")
            {
                ViewBag.TopicCount = 0;
                toReturn = null;
            }
            else
            {
                toReturn = db.Forums.SingleOrDefault(f => f.Category == Category);
            }
            return View(toReturn);
        }

        public ActionResult AddTopic() {

            
            ViewBag.CategoryList = db.Specializations.Select(s => s.SpecializationName).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult AddTopic(ForumTopic forumTopic)
        {
            ViewBag.CategoryList = db.Specializations.Select(s => s.SpecializationName).ToList();

            if (ModelState.IsValid && forumTopic.Category != null && forumTopic.Category != "")
            {
                var toAddInto = db.Forums.Single(f => f.Category == forumTopic.Category);
                toAddInto.Topics.Add(forumTopic);
                db.SaveChanges();
                ViewBag.TopicAdded = "true";
            }
            return View(forumTopic);
        }

        
        [HttpPost]
        public ActionResult AddPost(string PostMessage, int ForumTopicId)
        {
            string UserName = Membership.GetUser().UserName;

            if (PostMessage != null && PostMessage != " ")
            {
                ForumPost post = new ForumPost();
                post.TimeStamp = DateTime.Now;
                post.PostMessage = PostMessage;
                post.Postedby = db.UserProfiles.SingleOrDefault(p => p.UserName == UserName);
                var toAddInto = db.ForumTopics.SingleOrDefault(p => p.ForumTopicId == ForumTopicId);
                toAddInto.Posts.Add(post);
                db.SaveChanges();
            }
            return RedirectToAction("TopicDetail", "Forum", new { id = ForumTopicId });
        }

        public ActionResult TopicDetail(int id)
        {
            return View(db.ForumTopics.Single(t => t.ForumTopicId == id));
        }

        public ActionResult Categories()
        {
            var categories = db.Specializations.ToList();
            return View(categories);
        }


        public ActionResult Search(string q)
        {
            IEnumerable<Forum> resultForum = null;
            IEnumerable<ForumTopic> resultTopic = null;
            IEnumerable<ForumPost> resultPost = null;

            if (q == "" || q == null || q == " ")
            {
                resultForum = db.Forums.ToList();
                resultTopic = db.ForumTopics.ToList();
            }else {
                resultForum = from f in db.Forums
                              where ((f.Category.Contains(q) || (f.Field.Contains(q))))
                              orderby f.Field
                              select f;

                resultTopic = from t in db.ForumTopics
                              where ((t.Name.Contains(q) || (t.Description.Contains(q))))
                              orderby t.Name
                              select t;
            }
            resultPost = from p in db.ForumPosts
                            where ((p.PostMessage.Contains(q) || (p.Postedby.FirstName.Contains(q) || (p.Postedby.LastName.Contains(q)))))
                            orderby p.PostMessage
                        select p;
            
            ViewBag.ResultForum = resultForum;
            ViewBag.ResultTopic = resultTopic;
            ViewBag.ResultPost = resultPost;
            ViewBag.ResultCount = resultTopic.Count() + resultForum.Count() + resultPost.Count();
            ViewBag.SearchString = q;
            ViewData["keywords"] = q;
            return View(resultTopic.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Forum forum)
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(int ForumId)
        {
            return View();
        }
    }
}
