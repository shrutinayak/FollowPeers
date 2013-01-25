using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FollowPeers.Models
{
    public class NoticeComment
    {
        public int NoticeCommentId { get; set; }

        public virtual Update update { get; set; }

        public int commenterId { get; set; }
        [ForeignKey("commenterId")]
        public UserProfile commenter { get; set; }
        public string message { get; set; }
        public DateTime time { get; set; }
    }
}