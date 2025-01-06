namespace SignalMe.Client.Models
{
    public class ClientContact
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? ContactFirstName { get; set; }
        public string? ContactLastName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactTelephone { get; set; }

    }
}
