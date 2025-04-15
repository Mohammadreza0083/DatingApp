using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace API.Controllers
{
    /// <summary>
    /// Controller for managing user-related operations
    /// Handles user profiles, photos, and member information
    /// </summary>
    [Authorize]
    public class UsersController 
        (IUnitOfWork repo, IMapper mapper, IPhotoServices photoServices) 
        : BaseApiController
    {
        /// <summary>
        /// Retrieves a paginated list of users based on specified parameters
        /// </summary>
        /// <param name="userParams">Filtering and pagination parameters</param>
        /// <returns>List of member DTOs with pagination information</returns>
        [HttpGet]
        public async Task<ActionResult<List<MembersDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            userParams.CurrentUsername = User.GetUsername();
            var users = await repo.UserRepository.GetAllMembersAsync(userParams);
            if(users.Count is 0) return NotFound("No users found");
            Response.AddPaginationHeader(users);
            return users;
        }

        /// <summary>
        /// Retrieves detailed information about a specific user
        /// </summary>
        /// <param name="username">Username of the user to retrieve</param>
        /// <returns>Member DTO containing user details</returns>
        [HttpGet("{username}")]
        public async Task<ActionResult<MembersDto>> GetUser(string username)
        {
            var currentUsername = User.GetUsername();
            var user = await repo.UserRepository.GetMemberAsync(username, 
                isCurrentUser: currentUsername == username);
            if(user == null) return NotFound("No user found");
            return user;
        }

        /// <summary>
        /// Updates user profile information
        /// </summary>
        /// <param name="memberUpdateDto">DTO containing updated user information</param>
        /// <returns>NoContent if successful, BadRequest if failed</returns>
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await repo.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user is null)
            {
                return BadRequest("No user found");
            }
            mapper.Map(memberUpdateDto, user);
            if (await repo.Complete())
            {
                return NoContent();
            }
            return BadRequest("Failed to update user");
        }

        /// <summary>
        /// Adds a new photo to the user's profile
        /// </summary>
        /// <param name="file">The photo file to upload</param>
        /// <returns>Photo DTO if successful, BadRequest if failed</returns>
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await repo.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user is null)
            {
                return BadRequest("Something went wrong");
            }
            
            var result = photoServices.AddPhotoAsync(file);

            if (result.Result.Error is not null)
            {
                return BadRequest(result.Result.Error.Message);
            }

            var photo = new Photo()
            {
                Url = result.Result.SecureUrl.AbsoluteUri,
                PublicId = result.Result.PublicId
            };
            user.Photos.Add(photo);

            if (await repo.Complete())
            {
                return CreatedAtAction(nameof(GetUser), 
                    new {username = user.UserName}, 
                    mapper.Map<PhotoDto>(photo));
            }
            
            return BadRequest("Failed to add photo");
        }

        /// <summary>
        /// Sets a photo as the main profile photo
        /// </summary>
        /// <param name="photoId">ID of the photo to set as main</param>
        /// <returns>NoContent if successful, BadRequest if failed</returns>
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult<PhotoDto>> SetMainPhoto(int photoId)
        {
            var user = await repo.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user is null)
            {
                return BadRequest("No user found");
            }
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo is null || photo.IsMain)
            {
                return BadRequest("Can't set main photo");
            }
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain is not null)
            {
                currentMain.IsMain = false;
            }
            photo.IsMain = true;
            
            if (await repo.Complete())
            {
                return NoContent();
            }
            return BadRequest("Failed to set main photo");
        }

        /// <summary>
        /// Deletes a photo from the user's profile
        /// </summary>
        /// <param name="photoId">ID of the photo to delete</param>
        /// <returns>NoContent if successful, BadRequest if failed</returns>
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await repo.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user is null)
            {
                return BadRequest("No user found");
            }
            var photo = await repo.PhotoRepository.GetPhotoByIdAsync(photoId);
            var photos = user.Photos;
            if (photo is null)
            {
                return BadRequest("No photo found");
            }
            if (photo.IsMain)
            {
                photo.IsMain = false;
                photos.Remove(photo);
                if (photos.FirstOrDefault() is not null)
                {
                    photos.FirstOrDefault()!.IsMain = true;
                }
            }
            if (photo.PublicId is not null)
            {
                var result = photoServices.DeletePhotoAsync(photo.PublicId);
                if (result.Result.Error is not null)
                {
                    return BadRequest(result.Result.Error.Message);
                }
            }
            user.Photos.Remove(photo);
            if (await repo.Complete())
            {
                return NoContent();
            }
            return BadRequest("Failed to delete photo");
        }
    }
}
