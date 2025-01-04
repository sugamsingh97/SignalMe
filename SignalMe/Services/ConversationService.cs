
using SignalMe.Data;
using SignalMe.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalMe.Components.Account;
using System.Security.Claims;

namespace SignalMe.Services
{
    public class ConversationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConversationService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// This Searches the conversation between the loggedin user and the Receiver.
        /// If the conversation already exists it return the conversation, else it creates one.
        /// </summary>
        /// <param name="receiverId"></param>
        /// <returns></returns>
        public async Task<ClientConversation> GetOrCreateConversation(string receiverId)
        {
            var loggedInUser = await GetLoggedinUserId();
            if (string.IsNullOrEmpty(loggedInUser))
            {
                return null;
            }

            var conversation = await FindExistingConversation(loggedInUser, receiverId);
            return conversation ?? await CreateNewConversation(loggedInUser, receiverId);
        }



        /// <summary>
        /// This serches if the conversation already exist between the user and the receiver.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="receiverId"></param>
        /// <returns></returns>
        private async Task<ClientConversation> FindExistingConversation(string userId, string receiverId)
        {
            var conversation = await _db.Conversations
                .Where(c => (c.UserId == userId && c.ReceiverId == receiverId) ||
                           (c.UserId == receiverId && c.ReceiverId == userId))
                .FirstOrDefaultAsync();

            return conversation != null ? ConvertToClientConversation(conversation) : null;
        }

        /// <summary>
        /// This creates a new conversation between user and receiver.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="receiverId"></param>
        /// <returns></returns>
        private async Task<ClientConversation> CreateNewConversation(string userId, string receiverId)
        {

            var conversation = new Conversation
            {
                UserId = userId,
                ReceiverId = receiverId
            };

            await _db.Conversations.AddAsync(conversation);
            await _db.SaveChangesAsync();

            return ConvertToClientConversation(conversation);
        }

        /// <summary>
        /// This checks the Id and updates the Chat deletion date for only the  person who has deleted
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task UpdateChatDeletionDate(int conversationId, string userId)
        {
            var conversation = await _db.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation != null)
            {
                if (conversation.UserId == userId)
                {
                    conversation.UserChatDeleteDate = DateTime.UtcNow;
                }
                else if (conversation.ReceiverId == userId)
                {
                    conversation.ReceiverChatDeleteDate = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync();
            }
        }

       /// <summary>
       /// This creates a new message and with conversationId.
       /// </summary>
       /// <param name="content"></param>
       /// <param name="conversationId"></param>
       /// <returns></returns>
        public async Task<Message> CreateMessage(string content, int conversationId)
        {
            var senderId = await GetLoggedinUserId();

            //check convo exists or not
            var conversation = await _db.Conversations
                .Where(c => (c.Id == conversationId))
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                
            }
            //then if no exist then create

            // the assign it to new message.

            var message = new Message
            {
                Content = content,
                CreatedDate = DateTime.Now,
                SenderId = senderId,
                ConversationId = conversationId
            };

            await _db.Messages.AddAsync(message);
            await _db.SaveChangesAsync();

            return message;
        }

        /// <summary>
        /// This gets all the messages
        /// </summary>
        /// <param name="receiverId"></param>
        /// <returns></returns>
        public async Task<List<ClientMessage>> GetConversationMessages(string receiverId)
        {
            var loggedInUser = await GetLoggedinUserId();
            var conversation = await FindExistingConversation(loggedInUser, receiverId);

            if (conversation == null)
                return new List<ClientMessage>();     

            List<ClientMessage> _messages = new List<ClientMessage>();

            List<Message> messages = await _db.Messages
                .Where(m => m.ConversationId == conversation.Id)               
                .OrderBy(m => m.CreatedDate)
                .ToListAsync();

            foreach (var message in messages) 
            {
                ClientMessage m = new ClientMessage
                {
                    Id = message.Id,
                    CreatedDate = message.CreatedDate,
                    Content = message.Content,
                    ConversationId = message.ConversationId,
                    SenderId = message.SenderId
                };

                _messages.Add(m);
            }

            return _messages;
        }


        // This gets the logged in user
        public async Task<string> GetLoggedinUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId ?? string.Empty;
        }

        /// <summary>
        /// This is a helper function which converts thhe Server Conversation model to client Model
        /// </summary>
        /// <param name="conversation"></param>
        /// <returns></returns>
        public ClientConversation ConvertToClientConversation(Conversation conversation)
        {
            ClientConversation _conversation = new ClientConversation()
            {
                Id = conversation.Id,
                ReceiverId = conversation.ReceiverId,
                UserId = conversation.UserId,
                UserChatDeleteDate = conversation.UserChatDeleteDate,
                ReceiverChatDeleteDate = conversation.ReceiverChatDeleteDate
            };
            return _conversation;
        }

        public ClientMessage ConvertToClientMessage (Message message)
        {
            ClientMessage clientMessage = new ClientMessage()
            {
                Id= message.Id,
                Content = message.Content,
                CreatedDate = message.CreatedDate,
                SenderId = message.SenderId,
                ConversationId = message.ConversationId
              
            };
            return clientMessage;
        }
    }

}
