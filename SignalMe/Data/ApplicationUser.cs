using Microsoft.AspNetCore.Identity;

namespace SignalMe.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LasttName { get; set; }
    }

}
