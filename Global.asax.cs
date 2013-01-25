using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using FollowPeers.Models;

namespace FollowPeers
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Profile",
                "Profile/Index/{id}", // URL with parameters 
                new { controller = "Profile", action = "Index"}
                );// Parameter defaults );
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            //System.Data.Entity.Database.SetInitializer(new MyInitializer());
            //var FollowPeersDB = new FollowPeersDBEntities();
            //FollowPeersDB.Database.Initialize(true);

            Database.SetInitializer(new SampleData());

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
        public class MyInitializer
           : DropCreateDatabaseIfModelChanges<FollowPeersDBEntities>
        {
        }
    }
}