using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FollowPeers.Models
{
    public class Group
    {
        public int GroupId { set; get; }
        public string Name { set; get; }
        public int OwnerId { set; get; } //UserProfileId of owner
        public virtual ICollection<UserProfile> Members { get; set; } //UserProfiles of Members
    }
}