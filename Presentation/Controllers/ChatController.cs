using Application.Interfaces.Services;
using Application.ViewModel;
using Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Presentation.HUBs;

namespace Presentation.Controllers
{
    [Authorize]
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


        [HttpPost]
        public async Task<ActionResult> SendMessage([FromForm] SendMessageViewModel messageViewModel)
        {
            if (ModelState.IsValid)
            {
                var sendMessageResponse = await _chatService.SendMessageAsync(messageViewModel);
                await _chatHubContext.Clients.User(messageViewModel.RecipientId).SendAsync("ReceiveMessage", sendMessageResponse);

                return Json(sendMessageResponse);
            }

            return BadRequest("Invalid message data.");
        }


        [HttpGet("GetMessages")]
        public async Task<IActionResult> GetMessages(int chatId)
        {
            var user = await userManager.GetUserAsync(User);
            var id = user.Id;
            var messages = await _chatService.GetMessagesAsync(chatId);
            if (messages != null)
            {
                await _chatService.MarkMessagesAsSeenAsync(chatId, id);
            }
            return Ok(messages);
        }

        [HttpPost("MarkMessagesAsSeen")]
        public async Task<IActionResult> MarkMessagesAsSeen(int chatId)
        {
            var user = await userManager.GetUserAsync(User);
            var id = user.Id;
            await _chatService.MarkMessagesAsSeenAsync(chatId, id);
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
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

            var currentUser = await userManager.GetUserAsync(User);
            ViewBag.currentUserId = currentUser.Id;
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> Chat(string userId)
        {
            var currentUser = await userManager.GetUserAsync(User);

            var isAdmin = await userManager.IsInRoleAsync(currentUser, "Admin");

            if (isAdmin)
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User ID must be provided for admins.");
                }
            }
            else
            {
                var adminUsers = await userManager.GetUsersInRoleAsync("Admin");
                var adminUser = adminUsers.FirstOrDefault();
                userId = adminUser?.Id;

                if (userId == null)
                {
                    return BadRequest("No admin user found.");
                }
            }

            var chat = await _chatService.EnsureChatExistsAsync(currentUser.Id, userId);

            if (chat != null)
            {

                if (chat.Messages != null)
                {
                    await _chatService.MarkMessagesAsSeenAsync(chat.ChatId, currentUser.Id);
                }

                ViewBag.currentUserId = currentUser.Id;
                ViewBag.currentUserRole = currentUser.Role;
            }

            return View(chat);
        }


    }
}
