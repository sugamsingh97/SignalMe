using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SignalMe.Client.Models
{
    public class ClientConversation
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        [Required]
        public string? UserId { get; set; }
        public virtual AppUser? User { get; set; }


        [ForeignKey("Receiver")]
        [Required]
        public string? ReceiverId { get; set; }
        public virtual AppUser? Receiver { get; set; }

        public DateTime? UserChatDeleteDate { get; set; }

        public DateTime? ReceiverChatDeleteDate { get; set; }
    }
}
