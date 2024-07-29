namespace Core.Model
{
    public class Chat
    {
        public int ChatId { get; set; }
        public string User1Id { get; set; }
        public string User2Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public ApplicationUser User1 { get; set; }
        public ApplicationUser User2 { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
