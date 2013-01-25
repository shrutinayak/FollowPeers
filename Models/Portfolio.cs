using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FollowPeers.Models
{
    public class Portfolio //Portfolio is for investors to feature a list of companies they have funded.
    {
        [Key]
        public int PortfolioId { get; set; }
        public int UserProfileId { set; get; }
        public string Name { get; set; }
        public string BusinessField { get; set; }
        public string Country { get; set; }
        public int Year { get; set; }
        public string Status { get; set; }
        public string Website { get; set; }
        public string MoreInfo { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}