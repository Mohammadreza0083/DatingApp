using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class PhotoRepository(DataContext context) : IPhotoRepository
{
    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotosAsync()
    {
        return await context.Photos
            .IgnoreQueryFilters()
            .Where(p => p.IsApproved == false)
            .Select(u => new PhotoForApprovalDto()
            {
                Id = u.Id,
                Url = u.Url,
                IsApproved = u.IsApproved,
                Username = u.AppUsers.UserName
            }).ToListAsync();
    }

    public async Task<Photo?> GetPhotoByIdAsync(int id)
    {
        return await context.Photos
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    public void RemovePhoto(Photo photo)
    {
        context.Photos.Remove(photo);
    }
}