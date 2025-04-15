using API.DTO;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

/// <summary>
/// AutoMapper profile configuration for mapping between entities and DTOs
/// </summary>
public class AutoMapperProfiles : Profile
{
    /// <summary>
    /// Initializes a new instance of the AutoMapperProfiles class and configures mapping profiles
    /// </summary>
    public AutoMapperProfiles()
    {
        // Map AppUsers to MembersDto with custom mappings for age and photo URL
        CreateMap<AppUsers, MembersDto>()
            .ForMember(m=>m.Age,o=>o.MapFrom(u=>u.DateOfBirth.CalculateAge()))
            .ForMember(m => m.PhotoUrl, 
                // ReSharper disable once NullableWarningSuppressionIsUsed
                o=> o.MapFrom(u=>u.Photos.FirstOrDefault(x=>x.IsMain)!.Url));
        
        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDto, AppUsers>();
        CreateMap<RegisterDto, AppUsers>();
        CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
        
        // Map Message to MessageDto with custom mappings for sender and recipient photo URLs
        CreateMap<Message, MessageDto>()
            .ForMember(d => d.SenderPhotoUrl, 
                // ReSharper disable once NullableWarningSuppressionIsUsed
                o => o.MapFrom(u => u.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url))
            .ForMember(d => d.RecipientPhotoUrl, 
                // ReSharper disable once NullableWarningSuppressionIsUsed
                o => o.MapFrom(u => u.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url));
        
        // Configure DateTime mappings to ensure UTC kind
        CreateMap<DateTime, DateTime>().ConvertUsing(s => DateTime.SpecifyKind(s, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(s => s.HasValue 
            ? DateTime.SpecifyKind(s.Value, DateTimeKind.Utc) : null);
    }
}