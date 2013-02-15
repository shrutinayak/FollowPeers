using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web;
using System;

namespace FollowPeers.Models
{
    public class Jobs
    {
        [Key]
        public int JobId { set; get; }
        public string City { set; get; }
        public string Country { set; get; }
        public DateTime publishDate { get; set; }
        public DateTime Enddate { get; set; }
        public string Requirements { get; set; }
        public string Email { get; set; }
        public string URL { get; set; }
        public string Experience { get; set; }
        public string Industry { get; set; }
        public string Other { get; set; }
        public string Jobfunction { get; set; }
        public int ownerId { set; get; }
        public string Company { set; get; }
        public string Title { set; get; }
        public string Type { set; get; }
        public string Description { set; get; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<Specialization> Specializations { get; set; }
    }
}