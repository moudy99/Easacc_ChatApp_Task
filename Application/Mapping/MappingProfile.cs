using Application.ViewModel;
using AutoMapper;
using Core.Model;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>();
            CreateMap<ApplicationUser, RegisterViewModel>();
        }
    }
}
