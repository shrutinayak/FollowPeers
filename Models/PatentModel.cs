using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FollowPeers.Models
{
    public class PatentModel
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "About is required.")]
        public string About { get; set; }

        [Required(ErrorMessage = "Ptwent Number is required.")]
        public string RefNo { get; set; }

        public DateTime? ApplyYear { get; set; }

        public DateTime? AproveYear { get; set; }

        public string Keyword { get; set; }

        public string Country { get; set; }

        public int Ranking { get; set; }

        public int ViewCount { get; set; }

        public virtual UserProfile UserProfile { get; set; }
    }
}