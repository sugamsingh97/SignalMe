using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalMe.Client.Models
{
    public class ClientMessage
    {
        public int? Id { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        public string? Content { get; set; }
       

        [ForeignKey("Sender")]
        [Required]
        public string? SenderId { get; set; }
        public virtual AppUser? Sender { get; set; }

        [ForeignKey("Conversation")]
        [Required]
        public int? ConversationId { get; set; }
        public virtual ClientConversation? Conversation { get; set; }
    }
}
