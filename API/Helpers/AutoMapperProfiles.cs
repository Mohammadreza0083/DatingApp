using API.DTO;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUsers, MembersDto>()
            .ForMember(m=>m.Age,o=>o.MapFrom(u=>u.DateOfBirth.CalculateAge()))
            .ForMember(m => m.PhotoUrl, 
                // ReSharper disable once NullableWarningSuppressionIsUsed
                o=> o.MapFrom(u=>u.Photos.FirstOrDefault(x=>x.IsMain)!.Url));
        
        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDto, AppUsers>();
        CreateMap<RegisterDto, AppUsers>();
        CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
        
        CreateMap<Message, MessageDto>()
            .ForMember(d => d.SenderPhotoUrl, 
                // ReSharper disable once NullableWarningSuppressionIsUsed
                o => o.MapFrom(u => u.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url))
            .ForMember(d => d.RecipientPhotoUrl, 
                // ReSharper disable once NullableWarningSuppressionIsUsed
                o => o.MapFrom(u => u.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url));
        CreateMap<DateTime, DateTime>().ConvertUsing(s => DateTime.SpecifyKind(s, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(s => s.HasValue 
            ? DateTime.SpecifyKind(s.Value, DateTimeKind.Utc) : null);
    }
}