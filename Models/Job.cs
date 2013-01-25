using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FollowPeers.Models
{
    public class Job
    {
        public int JobId { set; get; }
        public int ownerId { set; get; }
        public string Country { set; get; }
        public string Title { set; get; }
        public string Description { set; get; }
        public bool students { set; get; }
        public bool professors { set; get; }
        public bool investors { set; get; }
        public bool researchers { set; get; }
        public DateTime publishDate { set; get; }
        public virtual ICollection<UserProfile> UserProfiles { get; set; }
        public virtual ICollection<Specialization> Specializations { get; set; }
    }
}