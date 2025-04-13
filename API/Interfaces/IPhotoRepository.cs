using API.DTO;
using API.Entities;

namespace API.Interfaces;

public interface IPhotoRepository
{
    Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotosAsync();
    Task<Photo?> GetPhotoByIdAsync(int id);
    void RemovePhoto(Photo photo);
}