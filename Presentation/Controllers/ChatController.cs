using Application.ViewModel;
using Core.Model;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Presentation.HUBs;

namespace Presentation.Controllers
{
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly ApplicationDbContext _dbContext;

        public ChatController(IHubContext<ChatHub> chatHubContext, ApplicationDbContext dbContext)
        {
            this._chatHubContext = chatHubContext;
            this._dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageViewModel model)
        {
            if (ModelState.IsValid)
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

                _dbContext.Messages.Add(message);
                await _dbContext.SaveChangesAsync();

                // Notify recipient via SignalR
                await _chatHubContext.Clients.User(model.RecipientId).SendAsync("ReceiveMessage", message);

                return Ok();
            }

            return BadRequest("Invalid message data.");
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(int chatId)
        {
            var messages = await _dbContext.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return Json(messages);
        }

        [HttpPost]
        public async Task<IActionResult> MarkMessagesAsSeen(int chatId)
        {
            var messages = await _dbContext.Messages
                .Where(m => m.ChatId == chatId && !m.IsSeen)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsSeen = true;
            }

            _dbContext.Messages.UpdateRange(messages);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
