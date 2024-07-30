using Microsoft.AspNetCore.Identity;
namespace Core.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public DateTime LastSeen { get; set; }
        public bool IsOnline { get; set; }

        public string Role { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }
    }
}
