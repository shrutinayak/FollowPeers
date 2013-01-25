using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FollowPeers.Models
{
    public class Tier
    {
        public int TierId { set; get; }
        public int Level { set; get; }
        public int Email { set; get; }
        public int Phone { set; get; }
        public int Mobile { set; get; }
        public int Address { set; get; }
        public int Education { set; get; }
        public int Publication { set; get; }
        public int Patent { set; get; }
        public int Noticeboard { set; get; }
        public int AboutMe { set; get; }
        public virtual UserProfile UserProfile { get; set; }

    }
}