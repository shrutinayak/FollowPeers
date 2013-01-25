using System.Data.Entity;
using System.Data;

namespace FollowPeers.Models
{
    public class FollowPeersDBEntities :  DbContext 
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Update> Updates { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<NoticeComment> NoticeComments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<PatentModel> PatentModels { get; set; }
        public DbSet<PublicationModel> PublicationModels { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<Journal> Journals { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Conference> Conferences { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<ForumTopic> ForumTopics { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserTypes> UserTypes { get; set; }
    }
}