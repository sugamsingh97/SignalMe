using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SignalMe.Data
{
    public class Message
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }

        [Required]
        public string? Content { get; set; }

        [ForeignKey("Sender")]
        [Required]
        public string? SenderId { get; set; }
        public virtual ApplicationUser? Sender { get; set; }

        [ForeignKey("Receiver")]
        [Required]
        public string? ReceiverId { get; set; }
        public virtual ApplicationUser? Receiver { get; set; }
    }
}
