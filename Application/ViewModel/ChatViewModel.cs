namespace Application.ViewModel
{
    public class ChatViewModel
    {
        public int ChatId { get; set; }
        public string User1Id { get; set; }
        public string User2Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public ApplicationUserViewModel User1 { get; set; }
        public ApplicationUserViewModel User2 { get; set; }
        public ICollection<MessageViewModel> Messages { get; set; }
    }
}
