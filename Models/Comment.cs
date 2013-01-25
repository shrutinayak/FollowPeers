using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FollowPeers.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        public string ParentType { get; set; }
        [Required]
        public string ParentId { get; set; }
        public string SubmittedBy { get; set; }
        [Required]
        public string CommentBody { get; set; }
        public string TimeSent { get; set; }
        public bool Flagged { get; set; }
        public bool Starred { get; set; }
    }
}