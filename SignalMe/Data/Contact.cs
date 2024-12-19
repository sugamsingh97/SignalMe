using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SignalMe.Data
{
    public class Contact
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }        

        [ForeignKey("User")]
        [Required]
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("Friend")]
        [Required]
        public string? FriendId { get; set; }
        public virtual ApplicationUser? Friend { get; set; }
    }
}
