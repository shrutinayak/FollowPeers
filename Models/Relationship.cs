using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FollowPeers.Models
{
    public class Relationship
    {
        [Key]
        public int RelationShipId { get; set; }
        public string personAName { get; set; }
        public string personBName { get; set; }
        public int tier { get; set; }
        //A follows B
    }
}