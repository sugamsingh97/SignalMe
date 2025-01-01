using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SignalMe.Data
{
    public class AppUserContact
    {
        [Key]
        public string? OwnerId { get; set; }
        public string ContactList { get; set; } = "[]";

        // De-serialize JSON string to List of string IDs
        public List<string> GetContactIds() 
        {
            var contactIds = JsonSerializer.Deserialize<List<string>>(ContactList); 
            return contactIds ?? new List<string>();
        } 
        
        // Serialize List of string IDs back to JSON string
        public void SetContactList(List<string> ids) 
        { 
            ContactList = JsonSerializer.Serialize(ids); 
        } 
        
        // Add a single contact ID to the list
        public void AddContact(string contactId) 
        { 
            var ids = GetContactIds(); 
            ids.Add(contactId); 
            SetContactList(ids); 
        }
        // Ensure the contact list is initialized properly at sign-up
        public void InitializeContactList() 
        { 
            if (string.IsNullOrEmpty(ContactList) || ContactList == "[]") 
            { 
                ContactList = "[]"; 
                // Initialize to an empty JSON array if it's null or empty
            }
        }
    }
}
