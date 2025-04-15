using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Repository for managing photo-related operations
/// Implements IPhotoRepository interface
/// </summary>
public class PhotoRepository(DataContext context) : IPhotoRepository
{
    /// <summary>
    /// Retrieves all unapproved photos from the database
    /// Ignores query filters to include all photos regardless of approval status
    /// </summary>
    /// <returns>A collection of PhotoForApprovalDto objects</returns>
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

    /// <summary>
    /// Retrieves a photo by its ID
    /// Ignores query filters to include all photos regardless of approval status
    /// </summary>
    /// <param name="id">The ID of the photo to find</param>
    /// <returns>The photo if found, null otherwise</returns>
    public async Task<Photo?> GetPhotoByIdAsync(int id)
    {
        return await context.Photos
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Removes a photo from the database
    /// </summary>
    /// <param name="photo">The photo entity to remove</param>
    public void RemovePhoto(Photo photo)
    {
        context.Photos.Remove(photo);
    }
}