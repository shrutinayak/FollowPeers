using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FollowPeers.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; } //type 1: PHD, type 2: Masters, type 3: Bachelor, type 4: Others
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string About { get; set; }
        public int Link { get; set; } //to link to actual user profile if the user is already on the network, if doesn't exist default value of -1
        public virtual UserProfile UserProfile { get; set; } // one userprofile will have many students
    }
}