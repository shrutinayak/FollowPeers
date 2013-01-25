using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FollowPeers.Models
{
    public class Specialization
    {
        public int SpecializationId { get; set; }
        public string Field { get; set; }
        public string SpecializationName { get; set; }
        public virtual ICollection<UserProfile> UserProfiles { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}