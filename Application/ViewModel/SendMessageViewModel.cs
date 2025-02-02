﻿using Microsoft.AspNetCore.Http;

namespace Application.ViewModel
{
    public class SendMessageViewModel
    {
        public string ChatId { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public string Content { get; set; }
        public string SentAt { get; set; }

        public IFormFile? img { get; set; }
        public IFormFile? voice { get; set; }

        public IFormFile? document { get; set; }



    }
}
