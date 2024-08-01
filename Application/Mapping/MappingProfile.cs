using Application.ViewModel;
using AutoMapper;
using Core.Model;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IsOnline, opt => opt.Ignore())
                .ForMember(dest => dest.LastSeen, opt => opt.Ignore());

            CreateMap<ApplicationUser, RegisterViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserName));

            CreateMap<Message, MessageViewModel>()
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.Recipient))
                .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.SentAt.ToLocalTime()));

            CreateMap<MessageViewModel, Message>()
                .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
                .ForMember(dest => dest.RecipientId, opt => opt.MapFrom(src => src.RecipientId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.SentAt.ToUniversalTime()))
                .ForMember(dest => dest.IsSeen, opt => opt.MapFrom(src => src.IsSeen))
                .ForMember(dest => dest.Chat, opt => opt.Ignore())
                .ForMember(dest => dest.Sender, opt => opt.Ignore())
                .ForMember(dest => dest.Recipient, opt => opt.Ignore());

            CreateMap<Chat, ChatViewModel>()
                .ForMember(dest => dest.User1, opt => opt.MapFrom(src => src.User1))
                .ForMember(dest => dest.User2, opt => opt.MapFrom(src => src.User2))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages));

            CreateMap<ChatViewModel, Chat>()
                .ForMember(dest => dest.User1, opt => opt.MapFrom(src => src.User1))
                .ForMember(dest => dest.User2, opt => opt.MapFrom(src => src.User2))
                .ForMember(dest => dest.Messages, opt => opt.Ignore())
                .ForMember(dest => dest.User1Id, opt => opt.MapFrom(src => src.User1Id))
                .ForMember(dest => dest.User2Id, opt => opt.MapFrom(src => src.User2Id));

            CreateMap<ApplicationUser, ApplicationUserViewModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => src.IsOnline));

            CreateMap<ApplicationUserViewModel, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => src.IsOnline));

            CreateMap<SendMessageViewModel, Message>()
          .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.ChatId))
          .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
          .ForMember(dest => dest.RecipientId, opt => opt.MapFrom(src => src.RecipientId))
          .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
          .ForMember(dest => dest.img, opt => opt.MapFrom(src => src.img))
          .ForMember(dest => dest.img, opt => opt.Ignore())
    .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => DateTime.Now));


            CreateMap<Message, SendMessageResponse>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.Name))
                .ForMember(dest => dest.RecipientName, opt => opt.MapFrom(src => src.Recipient.Name))
                .ForMember(dest => dest.IsRecipientOnline, opt => opt.MapFrom(src => src.Recipient.IsOnline));
        }
    }

}
