namespace Application.ViewModel
{
    public class MessageViewModel
    {
        public int MessageId { get; set; }
        public int ChatId { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsSeen { get; set; }

        public ApplicationUserViewModel Sender { get; set; }
        public ApplicationUserViewModel Recipient { get; set; }
    }
}
