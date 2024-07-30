using Application.Interfaces.Services;
using Application.ViewModel;
using Core.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Presentation.HUBs;

namespace Presentation.Controllers
{
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly IChatService _chatService;
        private readonly UserManager<ApplicationUser> userManager;

        public ChatController(IHubContext<ChatHub> chatHubContext, IChatService chatService, UserManager<ApplicationUser> userManager)
        {
            _chatHubContext = chatHubContext;
            _chatService = chatService;
            this.userManager = userManager;
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage(SendMessageViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _chatService.SendMessageAsync(model);

                await _chatHubContext.Clients.User(model.RecipientId).SendAsync("ReceiveMessage", model);

                return Ok();
            }

            return BadRequest("Invalid message data.");
        }

        [HttpGet("GetMessages")]
        public async Task<IActionResult> GetMessages(int chatId)
        {
            var messages = await _chatService.GetMessagesAsync(chatId);
            return Ok(messages);
        }

        [HttpPost("MarkMessagesAsSeen")]
        public async Task<IActionResult> MarkMessagesAsSeen(int chatId)
        {
            await _chatService.MarkMessagesAsSeenAsync(chatId);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> AdminDashboard()
        {
            var users = await userManager.Users
                .Select(u => new ApplicationUserViewModel
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    IsOnline = u.IsOnline,
                    Name = u.Name,
                }).ToListAsync();

            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> Chat(string userId)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var chat = await _chatService.EnsureChatExistsAsync(currentUser.Id, userId);
            var user = await userManager.GetUserAsync(User);
            if (chat != null)
            {
                ViewBag.currentUserId = user.Id;

            }
            return View(chat);
        }

    }
}
