using System;
using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    // AutoMapper helps us map from one object to another
    // We need to add this as another service in the AppServiceExtensions
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                // add the extra configuration to show the photoUrl and not null
                /* When we map an individual property, we give it the destination property; PhotoUrl
                   We tell it where want to map it from; Photos, and then the src of where this is 
                   located. It's in the Photos Collection and give the first Photo or that is default
                   that Is also Main (the user's main picture), and get the Url from that
                 */
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => 
                src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src =>
                 src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src =>
                 src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
        }
    }
}