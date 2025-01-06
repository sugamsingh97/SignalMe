
using SignalMe.Data;
using SignalMe.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalMe.Components.Account;
using System.Security.Claims;
using System.Linq;

namespace SignalMe.Services
{
    public class ConversationService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserService _userService;

        public ConversationService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext applicationDbContext, UserService userService)
        {
            _db = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }
        #region Conversation methods      
       
        public async Task<Conversation> CreateNewConversation(string receiverId)
        {
            string LoggedInUserId = await _userService.GetLoggedinUserId();            

            var _conversation = new Conversation
            {
                UserId = LoggedInUserId,
                ReceiverId = receiverId
            };

            await _db.Conversations.AddAsync(_conversation);
            await _db.SaveChangesAsync();
            return _conversation;
        }
        public async Task<Conversation?> FindExistingConversation(string userId, string receiverId)
        {
            var conversation = await _db.Conversations
                .Where(c => (c.UserId == userId && c.ReceiverId == receiverId) ||
                           (c.UserId == receiverId && c.ReceiverId == userId))
                .Include(c=>c.User)
                .Include(c=>c.Receiver)
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                return null;
            }

            return conversation;
        }
        public async Task<Conversation?> FindExistingConversation(int ConversationId)
        {
            var conversation = await _db.Conversations
                .Where(c => c.Id == ConversationId).FirstOrDefaultAsync();
            if (conversation == null)
            {
                return null;
            }
            return conversation;
        }
        public async Task<List<Conversation>?> GetAllConversations()
        {
            string loggedInUserId = await _userService.GetLoggedinUserId();
            var conversations = await _db.Conversations
                .Where(c => c.UserId == loggedInUserId || c.ReceiverId == loggedInUserId)
                .Include(c => c.User)
                .Include(c => c.Receiver)
                .ToListAsync();

            return conversations;
        }
        public async Task<bool> CheckConversationActiveStatus(string senderId, int conversationId)
        {
            var conversation = await FindExistingConversation(conversationId);
            if (senderId != conversation.UserId)
            {
                if (conversation.UserConversationIsActive == true)
                {
                    return true;
                }
                return false;
            }
            else if (senderId != conversation.ReceiverId)
            {
                if (conversation.ReceiverConversationIsActive == true)
                {
                    return true;
                }
                return false;
            }
            return false;

        }
        public async Task SetActiveConversation(int conversationId)
        {
            var loggedInUserId = await _userService.GetLoggedinUserId();

            var conversations = await _db.Conversations
                .Where(c => c.UserId == loggedInUserId || c.ReceiverId == loggedInUserId)
                .Include(c => c.User)
                .Include(c => c.Receiver)
                .ToListAsync();

            foreach (var c in conversations)
            {
                if (loggedInUserId == c.UserId && c.UserConversationIsActive == true)
                {
                    c.UserConversationIsActive = false;
                }
                else if (loggedInUserId == c.ReceiverId && c.ReceiverConversationIsActive == true)
                {
                    c.ReceiverConversationIsActive = false;
                }
            }

            var activeConversation = conversations.FirstOrDefault(c => c.Id == conversationId);
            if (activeConversation != null)
            {
                if (loggedInUserId == activeConversation.UserId)
                {
                    activeConversation.UserConversationIsActive = true;
                }
                else if (loggedInUserId == activeConversation.ReceiverId)
                {
                    activeConversation.ReceiverConversationIsActive = true;
                }
            }

            await _db.SaveChangesAsync();
        }
        public async Task<bool> IsConversationEmpty(int ConversationId)
        {
            var count = await _db.Messages
               .Where(m => m.ConversationId == ConversationId).CountAsync();
            return count > 0 ? false : true;

        }
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
        #endregion

        #region Message methods    
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
        public async Task<Message> CreateMessage(string content, int conversationId)
        {
            var senderId = await _userService.GetLoggedinUserId();

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

            var IsFriendConversationActive = await CheckConversationActiveStatus(senderId, conversationId);
            if (IsFriendConversationActive)
            {
                message.IsReadByReceiver = true;
            }

            await _db.Messages.AddAsync(message);
            await _db.SaveChangesAsync();

            return message;
        }
        public async Task<Message?> GetLastMessage(int ConversationId)
        {
            Message? lastMessage = await _db.Messages
                .Where(m => m.ConversationId == ConversationId)
                .OrderByDescending(m => m.CreatedDate).FirstOrDefaultAsync();
           
            return lastMessage;
        }
        // this toggles liked status
        public async Task ToggleLike(int messageId)
        {
            var message = await _db.Messages
       .Where(m => m.Id == messageId)
       .FirstOrDefaultAsync();             

            message.Liked = ! message.Liked;

            await _db.SaveChangesAsync();
        }
        public async Task<int> GetConsecutiveMessagingDays(int conversationId)
        {
            var today = DateTime.UtcNow.Date;
            var messages = await _db.Messages
                .Where(m => m.ConversationId == conversationId)
                .Select(m => m.CreatedDate.Date)
                .Distinct()
                .OrderByDescending(date => date)
                .ToListAsync();

            if (!messages.Any() || messages[0] != today)
                return 0;

            int consecutiveDays = 1;
            for (int i = 1; i < messages.Count; i++)
            {
                if (messages[i] == messages[i - 1].AddDays(-1))
                    consecutiveDays++;
                else
                    break;
            }

            return consecutiveDays;
        }
        
        public async Task<int?> GetUnreadCount(int ConversationId)
        {           
            string LoggedInUserId = await _userService.GetLoggedinUserId();

            var UnreadCount = await _db.Messages
                .Where(m => m.ConversationId == ConversationId
                    && m.SenderId != LoggedInUserId
                    && m.IsReadByReceiver == false).CountAsync();            

            return UnreadCount;
        }       

        public async Task ChangeReadStatus(int ConversationId)
        {
            //only change the read status of the messages which are not send by the logged in user
            //and if the read status are false
            string LoggedInUserId = await _userService.GetLoggedinUserId();

            var unreadMessages = await _db.Messages
        .Where(m => m.ConversationId == ConversationId
               && m.SenderId != LoggedInUserId
               && m.IsReadByReceiver == false)
        .ToListAsync();

            foreach (var message in unreadMessages)
            {
                message.IsReadByReceiver = true;
            }

            await _db.SaveChangesAsync();
        }
        #endregion

        #region Model Conversion
        public async Task<ClientConversation> ConvertToClientConversation(Conversation conversation)
        {
            ClientConversation _conversation = new();
            var loggedInUser = await _userService.GetLoggedinUserId();
            
            if (conversation.UserId == loggedInUser)
            {
                _conversation.Id = conversation.Id;
                _conversation.LoggedInUserId = loggedInUser;
                _conversation.FriendId = conversation.ReceiverId;
                _conversation.FriendFirstName = conversation.Receiver.FirstName;
                _conversation.FriendLastName = conversation.Receiver.LasttName;
            }
            else if (conversation.ReceiverId == loggedInUser)
            {
                _conversation.Id = conversation.Id;
                _conversation.LoggedInUserId = loggedInUser;
                _conversation.FriendId = conversation.UserId;
                _conversation.FriendFirstName = conversation.User.FirstName;
                _conversation.FriendLastName = conversation.User.LasttName;
            }
            _conversation.Streak = await GetConsecutiveMessagingDays(conversation.Id);
            _conversation.UnreadCount = await GetUnreadCount(conversation.Id);
            var LastMessage = await GetLastMessage(conversation.Id);
            if (LastMessage != null )
            {
                _conversation.LastMessage = LastMessage.Content;
                _conversation.LastMessageDate = LastMessage.CreatedDate;
            }            

            return _conversation;
        }
        public async Task<List<ClientMessage>> GetConversationMessages(string receiverId)
        {
            //getting the logged in user Id
            var loggedInUser = await _userService.GetLoggedinUserId();

            //seeing if the existing conversation exists
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
                    SenderId = message.SenderId,
                    IsReadByReceiver = message.IsReadByReceiver,
                    Liked = message.Liked
                    
                };

                _messages.Add(m);
            }

            return _messages;
        }
        #endregion

        #region Hub functions
        //get all conversations and then convert then into client models
        public async Task<List<ClientConversation>> GetAllClientConversations()
        {
            var conversations = await GetAllConversations();
            List<ClientConversation>? _conversationList = new List<ClientConversation>();
            if (conversations == null)
            {
                return _conversationList;
            }
            foreach (var c in conversations)
            {
                ClientConversation convo = await ConvertToClientConversation(c);
                _conversationList.Add(convo);
            }
            return _conversationList;
        }
        #endregion
    }
}
