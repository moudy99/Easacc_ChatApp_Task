using Core.Model;

namespace Application.Interfaces.Repository
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetMessagesByChatIdAsync(int chatId);
        Task<Message> GetMessageByIdAsync(int messageId);
        Task AddMessageAsync(Message message);
        Task UpdateMessageAsync(Message message);
        Task DeleteMessageAsync(int messageId);
    }
}
