using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SignalMe.Client.Models;
using SignalMe.Services;
namespace SignalMe.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserService _userService;
        private readonly ConversationService _conversationService;

        public ChatHub(UserService userService)
        {
            _userService = userService;
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task<List<AppUser>> GetUserList()
        {           
            var users = await _userService.GetUserList();
            return users.Value;
        }

    }
}
