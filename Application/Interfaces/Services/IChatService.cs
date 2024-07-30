using Application.ViewModel;
using Core.Model;

namespace Application.Interfaces.Services
{
    public interface IChatService
    {
        Task<Message> SendMessageAsync(SendMessageViewModel model);
        Task UpdateOnlineStatusAsync(string userId, bool isOnline);
        Task<ChatViewModel> GetChatAsync(string user1Id, string user2Id);
        Task<IEnumerable<MessageViewModel>> GetMessagesAsync(int chatId);
        Task MarkMessagesAsSeenAsync(int chatId);
        public Task<ChatViewModel> EnsureChatExistsAsync(string user1Id, string user2Id);

    }
}
