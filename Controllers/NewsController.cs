using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using FollowPeers.Models;
using System.Web.Security;

namespace FollowPeers.Controllers
{

    public class NewsController : Controller
    {
        FollowPeersDBEntities followPeersDB = new FollowPeersDBEntities();
        public static IEnumerable<Rss> GetRssFeed(List<string> q,string myreader)
        {
            List<Rss> newlist = new List<Rss>();
            foreach (string query in q)
            {
                string URL = "";
                if (myreader == "Yahoo") { URL = "http://search.news.yahoo.com/rss?p=" + query; }
                else { URL = "http://api.bing.com/rss.aspx?Source=News&Market=en-SG&Version=2.0&Query=" + query; }
                XDocument feedXml = XDocument.Load(URL);
                

                IEnumerable<Rss> temp = from feed in feedXml.Descendants("item")
                                        select new Rss
                                        {
                                            Area = query,
                                            Title = feed.Element("title").Value,
                                            Link = feed.Element("link").Value,
                                            Description = Regex.Match(feed.Element("description").Value, @"^.{1,180}\b(?<!\s)").Value
                                        };
                foreach (var i in temp)
                {
                    newlist.Add(i);
                }
            }
            return newlist;
        }


        public ActionResult Index(string reader)
        {
            string name = Membership.GetUser().UserName;
            UserProfile myprofile = followPeersDB.UserProfiles.First(p => p.UserName == name);
           
            List<string> q= new List<string>();
            if ((myprofile.Keywords.Count()==0) || (myprofile.Keywords.Count() == 1 && myprofile.Keywords.ElementAt(0).Area == ""))
            {
               // myprofile.Keywords.ElementAt(0).Area = "World News";
               //B followPeersDB.SaveChanges();
                q.Add("World News");
            }
            else
            {
                for (int i = 0; i < myprofile.Keywords.Count(); i++)
                {
                    q.Add(myprofile.Keywords.ElementAt(i).Area);
                }
            }
            ViewBag.reader = reader;
            ViewBag.query = q;
            return View(GetRssFeed(q,reader));
        }

    }
}
