using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalMe.Data
{
    public class Conversation
    {
        public int Id { get; set; }      

        [ForeignKey("User")]
        [Required]
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        

        [ForeignKey("Receiver")]
        [Required]
        public string? ReceiverId { get; set; }
        public virtual ApplicationUser? Receiver { get; set; }

        public DateTime? UserChatDeleteDate { get; set; }

        public DateTime? ReceiverChatDeleteDate { get; set; }

    }
}
