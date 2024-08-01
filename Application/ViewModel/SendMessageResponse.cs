namespace Application.ViewModel
{
    public class SendMessageResponse
    {
        public int MessageId { get; set; }
        public int ChatId { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsSeen { get; set; }
        public string? Img { get; set; }
        public string? Document { get; set; }
        public string? Voice { get; set; }
        public string SenderName { get; set; }
        public string RecipientName { get; set; }
        public bool IsRecipientOnline { get; set; }
    }
}
