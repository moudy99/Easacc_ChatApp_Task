using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel
{
    public class SendMessageViewModel
    {
        [Required]
        public int ChatId { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string RecipientId { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
