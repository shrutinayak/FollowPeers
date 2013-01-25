using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using FollowPeers.Models;

namespace FollowPeers.Models
{
    public class Forum
    {
        [Key]
        public int ForumId { get; set; }
        public string Field { get; set; }
        [Required]
        public string Category { get; set; }
        public virtual ICollection<ForumTopic> Topics { get; set; }
    }

    public class ForumTopic
    {
        [Key]
        public int ForumTopicId { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public virtual ICollection<ForumPost> Posts { get; set; }

    }
    public class ForumPost
    {
        [Key]
        public int ForumPostId { get; set; }
        [Required]
        public string PostMessage { get; set; }
        public virtual UserProfile Postedby { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}