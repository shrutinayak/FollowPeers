using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FollowPeers.Models
{
    public class UserTypes
    {
        [Key]
        public int UserTypeId { get; set; }
        public string Name { get; set; }
    }
}