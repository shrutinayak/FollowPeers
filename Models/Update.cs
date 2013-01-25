using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FollowPeers.Models
{
    public class Update
    {
        public int UpdateId { get; set; }
        public virtual ICollection<UserProfile> UserProfiles { get; set; }
        public Boolean New { get; set; } //to denote whether update has been viewed or not
        public Boolean Own { get; set; } //to denote own's update or not
        public DateTime Time { get; set; }
        public string message { get; set; }
        public string link { get; set; }
        public int owner { get; set; } //updater's Uerprofile
        public int type { get; set; } // 1: profile, 2: status msg update, 3: comment, 5:posttonoticeboard,
    }
}