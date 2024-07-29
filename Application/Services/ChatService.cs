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
        private readonly IUserService userService;

        public ChatService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this.userService = userService;
        }

        public async Task SendMessageAsync(SendMessageViewModel model)
        {
            var message = new Message
            {
                ChatId = model.ChatId,
                SenderId = model.SenderId,
                RecipientId = model.RecipientId,
                Content = model.Content,
                SentAt = DateTime.UtcNow,
                IsSeen = false
            };

            await _unitOfWork.MessageRepository.AddMessageAsync(message);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task UpdateOnlineStatusAsync(string userId, bool isOnline)
        {
            var user = await userService.FindByEmailAsync(userId);
            if (user != null)
            {
                user.IsOnline = isOnline;
                user.LastSeen = DateTime.UtcNow;

                userService.UpdateUser(user);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<ChatViewModel> GetChatAsync(string user1Id, string user2Id)
        {
            var chat = await _unitOfWork.ChatRepository.GetChatByUsersAsync(user1Id, user2Id);
            return _mapper.Map<ChatViewModel>(chat);
        }
    }
}
