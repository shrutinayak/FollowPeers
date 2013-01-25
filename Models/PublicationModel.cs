using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FollowPeers.Models
{
    public class PublicationModel
    {
        [Key]
        public int publicationID { set; get; }
        public int ownerID { set; get; }

        [Required(ErrorMessage = "Title is required.")]
        public string title { set; get; }

        //list
        public string status { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        public string author { get; set; }
        //type Journal Article , Book , Conferance , Thesis
        public string type { set; get; }

        public string journal { get; set; }
        public string publisher { get; set; }
        public string conference { get; set; }
        public string university { get; set; }

   
        public string referenceID { get; set; }
        public int year { get; set; }

        //issue of the jounal article or book
        public string issue { get; set; }
        public string volume { get; set; }
        public string page { get; set; }

        [Required(ErrorMessage = "Keyword is required.")]
        public string keyword { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public string description { get; set; }

        public int viewCount { get; set; }

        public virtual UserProfile UserProfile { get; set; }

        public PublicationModel()
        {
            title = " ";
            status = "Published";
            author = " ";
            type = "Book";
            journal = "N.A.";
            publisher = "N.A.";
            conference = "N.A.";
            university = "N.A.";
            referenceID = "";
            year = 1900;
            issue = " ";
            volume = " ";
            page = " ";
            keyword = " ";
            description = " ";
            viewCount = 0;
        }
    }
}