using API.DTO;
using API.Entities;

namespace API.Interfaces;

/// <summary>
/// Interface for managing photos and photo approvals
/// </summary>
public interface IPhotoRepository
{
    /// <summary>
    /// Gets a list of photos waiting for approval
    /// </summary>
    /// <returns>A list of photo approval DTOs</returns>
    Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotosAsync();

    /// <summary>
    /// Gets a photo by its ID
    /// </summary>
    /// <param name="id">The ID of the photo</param>
    /// <returns>The photo if found, null otherwise</returns>
    Task<Photo?> GetPhotoByIdAsync(int id);

    /// <summary>
    /// Removes a photo from the repository
    /// </summary>
    /// <param name="photo">The photo to remove</param>
    void RemovePhoto(Photo photo);
}