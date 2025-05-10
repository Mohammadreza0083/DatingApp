using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services;

/// <summary>
/// Service for managing photo operations with Cloudinary
/// </summary>
public class PhotoServices : IPhotoServices
{
    private readonly Cloudinary _cloudinary;

    /// <summary>
    /// Initializes a new instance of the PhotoServices class
    /// </summary>
    /// <param name="config">The Cloudinary configuration settings</param>
    public PhotoServices(IOptions<CloudinarySettings> config)
    {
        var acc = new Account
            (config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
        _cloudinary = new Cloudinary(acc);
    }

    /// <summary>
    /// Uploads a photo to Cloudinary
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <returns>The result of the upload operation</returns>
    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();
        if (file.Length > 0)
        {
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.Name, stream),
                Transformation = new Transformation()
                    .Height(500).Width(500).Crop("fill").Gravity("face")
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }

    /// <summary>
    /// Deletes a photo from Cloudinary
    /// </summary>
    /// <param name="publicId">The public ID of the photo to delete</param>
    /// <returns>The result of the deletion operation</returns>
    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deleteParams);
    }
}