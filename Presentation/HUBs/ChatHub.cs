using Core.Model;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Presentation.HUBs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;

        public ChatHub(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            var connectionId = Context.ConnectionId;

            await _dbContext.UserConnections.AddAsync(new UserConnection
            {
                ConnectionId = connectionId,
                UserId = userId,
                ConnectedAt = DateTime.UtcNow
            });

            await UpdateOnlineStatus(userId, true);
            await _dbContext.SaveChangesAsync();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            var userConnection = await _dbContext.UserConnections
                .FirstOrDefaultAsync(uc => uc.ConnectionId == connectionId);

            if (userConnection != null)
            {
                var userConnections = await _dbContext.UserConnections
                    .Where(uc => uc.UserId == userConnection.UserId)
                    .ToListAsync();

                _dbContext.UserConnections.RemoveRange(userConnections);

                await _dbContext.SaveChangesAsync();

                await UpdateOnlineStatus(userConnection.UserId, false);
            }

            await base.OnDisconnectedAsync(exception);
        }


        private async Task UpdateOnlineStatus(string userId, bool isOnline)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsOnline = isOnline;
                user.LastSeen = DateTime.UtcNow;
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
            }
        }


    }
}
