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

        public ChatHub(UserService userService, ConversationService conversationService)
        {
            _userService = userService;
            _conversationService = conversationService;
        }
        public async Task SendMessage(string receiverId, string content)
        {

            var conversation = await _conversationService.GetOrCreateConversation(receiverId);
            if (conversation != null && conversation.Id != 0)
            {
                var message = await _conversationService.CreateMessage(content, conversation.Id);
                await Clients.Users(new[] { receiverId, message.SenderId }).SendAsync("ReceiveMessage", message);
            }
        }

        public async Task<List<AppUser>> GetUserList()
        {           
            var users = await _userService.GetUserList();
            return users.Value;
        }

        public async Task<ClientConversation> GetOrCreateConversation(string receiverId)
        {
            return await _conversationService.GetOrCreateConversation(receiverId);
        }

        public async Task DeleteChat(int conversationId)
        {
            var userId = await _conversationService.GetLoggedinUserId();
            await _conversationService.UpdateChatDeletionDate(conversationId, userId);
        }

        public async Task<List<ClientMessage>> GetConversationMessages(string receiverId)
        {
            List<ClientMessage> messages = new();
                messages = await _conversationService.GetConversationMessages(receiverId);
            return messages;
        }

        public async Task<AppUser?> SearchUser(string email)
        {
            var user = await _userService.SearchUserByEmail(email);
            return user;
        }

    }
}
