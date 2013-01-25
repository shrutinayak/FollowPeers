using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FollowPeers.Models
{
    public class Notification
    {
        public int NotificationID { get; set; }
        public string message { get; set; }
        public int type { get; set; }
        public string imagelink { get; set; } //for thumbnail image
        public string link { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public bool New {get;set;}
    }
}