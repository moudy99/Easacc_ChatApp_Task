using Application.Interfaces.Repository;
using Core.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Chat> GetChatByUsersAsync(string user1Id, string user2Id)
        {
            return await _context.Chats
                .Include(c => c.User1)
                .Include(c => c.User2)
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == user1Id && c.User2Id == user2Id) ||
                    (c.User1Id == user2Id && c.User2Id == user1Id));
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            return await _context.Chats
                .Include(c => c.User1)
                .Include(c => c.User2)
                .ToListAsync();
        }

        public async Task AddChatAsync(Chat chat)
        {
            await _context.Chats.AddAsync(chat);
        }

        public async Task UpdateChatAsync(Chat chat)
        {
            _context.Chats.Update(chat);
            await Task.CompletedTask;
        }

        public async Task DeleteChatAsync(int chatId)
        {
            var chat = await _context.Chats.FindAsync(chatId);
            if (chat != null)
            {
                _context.Chats.Remove(chat);
            }
        }
    }
}