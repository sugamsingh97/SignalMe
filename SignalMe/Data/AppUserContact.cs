using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace SignalMe.Data
{
    public class AppUserContact
    {
        public int Id { get; set; }

        [ForeignKey("Owner")]
        [Required]
        public string? OwnerId { get; set; }
        public virtual ApplicationUser? Owner { get; set; }


        [ForeignKey("Contact")]
        [Required]
        public string? ContactId { get; set; }
        public virtual ApplicationUser? Contact { get; set; }
    }
}
