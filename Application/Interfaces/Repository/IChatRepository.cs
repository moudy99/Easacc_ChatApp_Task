using Core.Model;

namespace Application.Interfaces.Repository
{
    public interface IChatRepository
    {
        Task<Chat> GetChatByUsersAsync(string user1Id, string user2Id);
        Task<IEnumerable<Chat>> GetAllChatsAsync();
        Task AddChatAsync(Chat chat);
        Task UpdateChatAsync(Chat chat);
        Task DeleteChatAsync(int chatId);
    }
}
