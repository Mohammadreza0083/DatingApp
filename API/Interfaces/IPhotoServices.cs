using CloudinaryDotNet.Actions;

namespace API.Interfaces;

/// <summary>
/// Interface for managing photo operations with Cloudinary
/// </summary>
public interface IPhotoServices
{
    /// <summary>
    /// Uploads a photo to Cloudinary
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <returns>The result of the upload operation</returns>
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
    
    /// <summary>
    /// Deletes a photo from Cloudinary
    /// </summary>
    /// <param name="publicId">The public ID of the photo to delete</param>
    /// <returns>The result of the deletion operation</returns>
    Task<DeletionResult> DeletePhotoAsync(string publicId);
}