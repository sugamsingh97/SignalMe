using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SignalMe.Client.Models
{
    public class ClientConversation
    {
        public int Id { get; set; }

        [ForeignKey("LoggedInUser")]
        [Required]
        public string? LoggedInUserId { get; set; }
        public virtual AppUser? LoggedInUser { get; set; }


        [ForeignKey("Friend")]
        [Required]
        public string? FriendId { get; set; }
        public virtual AppUser? Friend { get; set; }

        public string? FriendFirstName { get; set; }
        public string? FriendLastName { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public int? Streak { get; set; }
        public int? UnreadCount { get; set; }
        public string? LastMessage { get; set; }   

        public DateTime? LastMessageDate { get; set; }
        public DateTime? UserChatDeleteDate { get; set; }

        public DateTime? ReceiverChatDeleteDate { get; set; }
    }
}
