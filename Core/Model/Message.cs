namespace Core.Model
{
    public class Message
    {
        public int MessageId { get; set; }
        public int ChatId { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsSeen { get; set; }
        public Chat Chat { get; set; }
        public string? img { get; set; }
        public ApplicationUser Sender { get; set; }
        public ApplicationUser Recipient { get; set; }
    }
}
