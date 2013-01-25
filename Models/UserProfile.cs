using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web;
using System;

namespace FollowPeers.Models
{
    
    public class UserProfile
    {
        //Basic info: userId, username, name, languages, thumbnail, profilepic, age, gender, description, location
        [ScaffoldColumn(false)]
        [Key]
        public int UserProfileId { get; set; }
        [ScaffoldColumn(false)]
        public string UserName { get; set; }
        [DisplayName("First Name")]
      //  [Required(ErrorMessage = "Your First Name please.")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string Profession { get; set; }
        public string PhotoUrl { get; set; }
    //    [Required(ErrorMessage = "Your Age please. We're all young at heart. Yes.")]
       
        public DateTime? Birthday { get; set; }
    //    [Required(ErrorMessage = "Your Gender please.")]
        public string Gender { get; set; }
        public string Status { get; set; }
        public string StatusMessage { get; set; }
      //  [Required(ErrorMessage = "Where are you located again?")]
        public int Default { get; set; }
        [ScaffoldColumn(false)]
        public float SignUpProgress { get; set; }
        [ScaffoldColumn(false)]
        public bool activated { get; set; }
        public bool firsttime { get; set; }
        public string Organization { get; set; }
        public string Departments { get; set; }
        public string Country { get; set; }
        [AllowHtml]
        public string AboutMe { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual ICollection<Education> Educations { get; set; }
        public virtual ICollection<PublicationModel> Publication { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<PatentModel> Patent { get; set; }
        public virtual ICollection<Update> Updates { get; set; }
        public virtual ICollection<Tier> Tiers { get; set; }
        public virtual ICollection<Specialization> Specializations { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<Keyword> Keywords { get; set; }
        public virtual ICollection<Conversation> Conversations { get; set; }
        public virtual ICollection<Portfolio> Portfolios { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public List<int> PublicationBookmark { get; set; }
    }
    public class Keyword
    {
        public int KeywordId { get; set; }
        public string Area { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}