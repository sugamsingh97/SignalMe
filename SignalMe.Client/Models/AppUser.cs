using System.ComponentModel.DataAnnotations;

namespace SignalMe.Client.Models
{
    public class AppUser
    {
        [Required]
        public string? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
    }
}
