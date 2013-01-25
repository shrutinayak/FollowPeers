using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FollowPeers.Models
{
    public class Education
    {
        public int EducationId { set; get; }
        public int UserProfileId { set; get; }
        public string UniversityName { set; get; }
        public int startYear { set; get; }
        public int endYear { set; get; }
        public string Degree { set; get; }
        public string country { set; get; }
        public virtual UserProfile UserProfile { get; set; }
    }
}