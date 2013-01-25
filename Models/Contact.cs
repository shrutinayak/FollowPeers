using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FollowPeers.Models
{
    public class Contact
    {
        public int ContactId { set; get; }
        public int UserProfileId { set; get; }
        public string Street1 { set; get; }
        public string Street2 { set; get; }
        public string City { set; get; }
        public string Email { set; get; }
        public string Phone { set; get; }
        public string Website { set; get; }
        public string Fax { set; get; }
        public string Country { set; get; }
        public string Mobile { set; get; }
    }
}