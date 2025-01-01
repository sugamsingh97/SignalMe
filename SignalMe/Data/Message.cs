using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SignalMe.Data
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string? Content { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("Sender")]
        [Required]
        public string? SenderId { get; set; }
        public virtual ApplicationUser? Sender { get; set; }        

        [ForeignKey("Conversation")]
        [Required]
        public int? ConversationId { get; set; }
        public virtual Conversation? Conversation { get; set; }
    }
}
