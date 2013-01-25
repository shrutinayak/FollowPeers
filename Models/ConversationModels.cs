using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FollowPeers.Models
{
    public class Conversation
    {
        public int ConversationId { get; set; }
        public List<string> Participants { get; set; }
        //public virtual List<Message> Message { get; set; }
    }

    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public string Sender { get; set; }
        [Required]
        public string Reciever { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string MessageBody { get; set; }
        public string TimeSent { get; set; }
        public bool Trashed { get; set; }
        public bool Starred { get; set; }
    }

    public class NewConversationModel
    {
        [Required]
        public string Participants { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string MessageBody { get; set; }
        [Required]
        public DateTime TimeSent { get; set; }
    }
}