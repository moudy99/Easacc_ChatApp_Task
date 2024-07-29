namespace Core.Model
{
    public class UserConnection
    {
        public int Id { get; set; }
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        public DateTime ConnectedAt { get; set; }

        public ApplicationUser User { get; set; }
    }
}
