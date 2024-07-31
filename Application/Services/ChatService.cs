using Application.Helpers;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Application.ViewModel;
using AutoMapper;
using Core.Model;

namespace Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public ChatService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<Message> SendMessageAsync(SendMessageViewModel model)
        {
            var message = _mapper.Map<Message>(model);
            if (model.img != null)
            {
                var imgUrl = await ImageSavingHelper.SaveOneImageAsync(model.img, "chatImages");
                message.img = imgUrl;
            }

            if (model.document != null)
            {
                var documentUrl = await DocumentSavingHelper.SaveOneDocumentAsync(model.document, "documents");
                message.document = documentUrl;
            }
            message.SentAt = DateTime.UtcNow;
            message.IsSeen = false;

            await _unitOfWork.MessageRepository.AddMessageAsync(message);
            await _unitOfWork.SaveChangesAsync();
            return message;
        }

        public async Task UpdateOnlineStatusAsync(string userId, bool isOnline)
        {
            var user = await _userService.FindByEmailAsync(userId);
            if (user != null)
            {
                user.IsOnline = isOnline;
                user.LastSeen = DateTime.UtcNow;

                await _userService.UpdateUser(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<ChatViewModel> GetChatAsync(string user1Id, string user2Id)
        {
            var chat = await _unitOfWork.ChatRepository.GetChatByUsersAsync(user1Id, user2Id);
            return _mapper.Map<ChatViewModel>(chat);
        }

        public async Task<IEnumerable<MessageViewModel>> GetMessagesAsync(int chatId)
        {
            var messages = await _unitOfWork.MessageRepository.GetMessagesByChatIdAsync(chatId);
            return _mapper.Map<IEnumerable<MessageViewModel>>(messages);
        }

        public async Task MarkMessagesAsSeenAsync(int chatId)
        {
            var messages = await _unitOfWork.MessageRepository.GetMessagesByChatIdAsync(chatId);
            var unseenMessages = messages.Where(m => !m.IsSeen).ToList();

            foreach (var message in unseenMessages)
            {
                message.IsSeen = true;
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ChatViewModel> EnsureChatExistsAsync(string user1Id, string user2Id)
        {
            var chat = await _unitOfWork.ChatRepository.GetChatByUsersAsync(user1Id, user2Id);

            if (chat == null)
            {
                chat = new Chat
                {
                    User1Id = user1Id,
                    User2Id = user2Id,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.ChatRepository.AddChatAsync(chat);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<ChatViewModel>(chat);
        }

    }
}
