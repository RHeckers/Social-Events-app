using System.Linq;
using AutoMapper;
using Domain;

namespace Application.Activities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mappping configurations for AutoMapper <From, To>
            CreateMap<Activity, ActivityDto>();
            CreateMap<UserActivity, AttendeeDto>()
                        .ForMember(d => d.Username, opt => opt.MapFrom(src => src.AppUser.UserName))
                        .ForMember(d => d.Image, opt => opt.MapFrom(src => src.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url))
                        .ForMember(d => d.DisplayName, opt => opt.MapFrom(src => src.AppUser.DisplayName));
        }
    }
}