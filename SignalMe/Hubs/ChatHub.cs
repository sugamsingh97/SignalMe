using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SignalMe.Client.Models;
using SignalMe.Data;
using SignalMe.Services;
namespace SignalMe.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserService _userService;
        private readonly ConversationService _conversationService;
        private readonly ContactService _contactService;

        public ChatHub(UserService userService, ConversationService conversationService, ContactService contactService)
        {
            _userService = userService;
            _conversationService = conversationService;
            _contactService = contactService;
        }

        #region Users
        public async Task<List<AppUser?>> GetUserList()
        {
            var users = await _userService.GetUserList();
            return users.Value;
        }
        public async Task<AppUser?> SearchUser(string email)
        {
            var user = await _userService.SearchUserByEmail(email);
            return user;
        }
        #endregion

        #region Messages
        public async Task SendMessage(string receiverId, string content)
        {
            // find existing conversation between the two
            string loggedInUser = await _userService.GetLoggedinUserId();

            // if the conversation exists return the conversation
            var conversation = await _conversationService.FindExistingConversation(loggedInUser, receiverId);

            // if the result is null then create a new conversation
            if (conversation == null)
            {
                conversation = await _conversationService.CreateNewConversation(receiverId);
            }

            if (conversation != null && conversation.Id != 0)
            {
                var message = await _conversationService.CreateMessage(content, conversation.Id);
                await Clients.Users(new[] { receiverId, message.SenderId }).SendAsync("ReceiveMessage", message);
                await Clients.Users(new[] { receiverId, message.SenderId }).SendAsync("ConversationUpdated");
            }
        }
        public async Task ChangeReadStatus(int conversationId)
        {
            //find the conversation
            var conversation = await _conversationService.FindExistingConversation(conversationId);

            //this changes the read status
            await _conversationService.ChangeReadStatus(conversationId);

            // Notify only the users involved in this conversation
            await Clients.Users(new[] { conversation.UserId, conversation.ReceiverId })
                .SendAsync("ReadStatusChanged", conversationId);
        }

        public async Task ToggleLike(int messageId, int conversationId)
        {
            //find the conversation
            var conversation = await _conversationService.FindExistingConversation(conversationId);

            //This updates the like 
            await _conversationService.ToggleLike(messageId);
            
            //This notifies the frontend
            await Clients.Users(new[] { conversation.UserId, conversation.ReceiverId })
                .SendAsync("LikeToggled", conversationId);
        }
        
        public async Task<List<ClientMessage>> GetConversationMessages(string receiverId)
        {
            List<ClientMessage> messages = new();
            messages = await _conversationService.GetConversationMessages(receiverId);
            return messages;
        }
        #endregion

        #region Conversation
        public async Task<List<ClientConversation>> GetAllConversation()
        {
            var conversations = await _conversationService.GetAllClientConversations();
            return conversations;
        }
        public async Task<int> GetOrCreateConversation(string receiverId)
        {          

            // find existing conversation between the two
            string loggedInUser = await _userService.GetLoggedinUserId();

            // if the conversation exists return the conversation
            var conversation = await _conversationService.FindExistingConversation(loggedInUser, receiverId);

            // if the result is null then create a new conversation
            if (conversation == null)
            {
                conversation = await _conversationService.CreateNewConversation(receiverId);
            }
            return conversation.Id;
        }
        public async Task DeleteChat(int conversationId)
        {
            var userId = await _userService.GetLoggedinUserId();
            await _conversationService.UpdateChatDeletionDate(conversationId, userId);
        }
        public async Task SetActiveConversation(int ConversationId)
        {
            var currentUserId = await _userService.GetLoggedinUserId();
            await _conversationService.SetActiveConversation(ConversationId);
            // Notify only the current user about their conversation status change
            await Clients.User(currentUserId).SendAsync("ConversationStatusChanged", ConversationId);
        }
        #endregion

        #region Contact
        public async Task CreateContact(string UserId)
        {
            var currentUserId = await _userService.GetLoggedinUserId();
            await _contactService.CreateNewContact(UserId);
            await Clients.User(currentUserId).SendAsync("ContactCreated");
        }
        public async Task<List<ClientContact>> GetAllUserContacts()
        {
            var contacts = await _contactService.GetAllUserContacts();
            return contacts;
        }
        #endregion



        
       

    }
}
