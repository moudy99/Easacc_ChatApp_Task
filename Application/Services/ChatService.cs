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

        public async Task<SendMessageResponse> SendMessageAsync(SendMessageViewModel model)
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

            if (model.voice != null)
            {
                var voiceUrl = await VoiceSavingHelper.SaveVoiceAsync(model.voice, "voices");
                message.voice = voiceUrl;
            }

            await _unitOfWork.MessageRepository.AddMessageAsync(message);
            await _unitOfWork.SaveChangesAsync();

            var fullMessage = await this.GetMessageAsync(message.MessageId);
            var sendMessageResponse = _mapper.Map<SendMessageResponse>(message);
            sendMessageResponse.IsRecipientOnline = fullMessage.Recipient.IsOnline;

            return sendMessageResponse;
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

        public async Task MarkMessagesAsSeenAsync(int chatId, string senderId)
        {
            var messages = await _unitOfWork.MessageRepository.GetMessagesByChatIdAsync(chatId);
            var unseenMessages = messages.Where(m => !m.IsSeen && m.SenderId != senderId).ToList();

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
                var user1 = await _userService.getUserById(user1Id);
                var user2 = await _userService.getUserById(user2Id);
                chat = new Chat
                {
                    User1Id = user1Id,
                    User2Id = user2Id,
                    User1 = user1,
                    User2 = user2,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.ChatRepository.AddChatAsync(chat);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<ChatViewModel>(chat);
        }

        public Task<Message> GetMessageAsync(int messageId)
        {
            var mes = _unitOfWork.MessageRepository.GetMessageByIdAsync(messageId);
            if (mes != null)
            {

                return mes;
            }
            return null;
        }

    }
}
