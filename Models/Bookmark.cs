using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FollowPeers.Models
{
    public class Bookmark
    {
        [Key]
        public int bookmarkID { get; set; }
        [Required]
        public string bookmarkType { get; set; }
        [Required]
        public int itemID { get; set; }
        [Required]
        public int userID { get; set; }
    }
}
