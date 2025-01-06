using SignalMe.Data;
using SignalMe.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace SignalMe.Services
{
    public class ContactService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserService _userService;
        public ContactService(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor, UserService userService)
        {
            _db = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        // Create new contact connection
        public async Task<ActionResult> CreateNewContact(string UserId)
        {
            // Check if contact exists
            if (!await _userService.UserExists(UserId))
            {
                return new BadRequestObjectResult("Contact not found");
            }

            var _contact = await GetContactByUserId(UserId);

            if (_contact != null)
            {
                return new BadRequestObjectResult("Contact Already Exists");
            }

            var ownerId = await _userService.GetLoggedinUserId();

            var contact = new AppUserContact
            {
                OwnerId = ownerId,
                ContactId = UserId
            };

            await _db.Contacts.AddAsync(contact);
            await _db.SaveChangesAsync();

            return new OkObjectResult(contact);
        }

        // Delete a contact connection

        public async Task<ActionResult> DeleteContact(string UserId)
        {
            var contact = await GetContactByUserId(UserId);
            if (contact == null)
            {
                return null;
            }
            else
            {
                _db.Contacts.Remove(contact);
                await _db.SaveChangesAsync();
            }

            return new OkResult();
        }

        // Get all the contacts from a logged in user

        public async Task<List<ClientContact>> GetAllUserContacts()
        {
            var userId = await _userService.GetLoggedinUserId();

            var contacts = await _db.Contacts
                .Include(c => c.Owner)
                .Include(c => c.Contact)
                .Where(c => c.OwnerId == userId).ToListAsync();
                    return (await ConvertToClientContact(contacts));
        }

        public async Task<AppUserContact> GetContactByUserId(string UserId)
        {
            var loggedInUserId = await _userService.GetLoggedinUserId();
            var contact = await _db.Contacts.Where(c => c.OwnerId == loggedInUserId && c.ContactId == UserId).FirstOrDefaultAsync();
            if (contact == null)
            {
                return null;
            }
            return contact;
        }

        public async Task<List<ClientContact>> ConvertToClientContact(List<AppUserContact> contacts)
        {
            List<ClientContact> _contacts = new List<ClientContact>();
            foreach (var contact in contacts)
            {
                ClientContact c = new ClientContact
                {
                    Id = contact.Id,
                    UserId = contact.ContactId,
                    ContactFirstName = contact.Contact.FirstName,
                    ContactLastName = contact.Contact.LasttName,
                    ContactEmail = contact.Contact.Email,
                    ContactTelephone = contact.Contact.PhoneNumber
                };
                _contacts.Add(c);
            }
            return _contacts;
        }
    }
}
