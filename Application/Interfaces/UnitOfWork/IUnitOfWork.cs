
using Application.Interfaces.Repository;

namespace Application.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();

        public IChatRepository ChatRepository { get; }
        public IMessageRepository MessageRepository { get; }
    }
}
