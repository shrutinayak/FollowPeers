using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FollowPeers.Models
{
    public class Event
    {
        [Key]
        public int EventId {get; set; }
        public string EventName {get; set;}
        public DateTime EventDateTime { get; set; }
        public string EventDesc { get; set; }
    }
}