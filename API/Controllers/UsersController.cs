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
    [Authorize]
    public class UsersController 
        (IUserRepository userRepository, IMapper mapper, IPhotoServices photoServices) 
        : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<List<MembersDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            userParams.CurrentUsername = User.GetUsername();
            var users = await userRepository.GetAllMembersAsync(userParams);
            if(users.Count is 0) return NotFound("No users found");
            Response.AddPaginationHeader(users);
            return users;
        }
        [HttpGet("{username}")]
        public async Task<ActionResult<MembersDto>> GetUser(string username)
        {
            var user = await userRepository.GetMemberAsync(username);
            if(user == null) return NotFound("No user found");
            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user is null)
            {
                return BadRequest("No user found");
            }
            mapper.Map(memberUpdateDto, user);
            if (await userRepository.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

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
            
            if (user.Photos.Count is 0)
            {
                photo.IsMain = true;
            }
            
            user.Photos.Add(photo);

            if (await userRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUser), 
                    new {username = user.UserName}, 
                    mapper.Map<PhotoDto>(photo));
            }
            
            return BadRequest("Failed to add photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult<PhotoDto>> SetMainPhoto(int photoId)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
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
            
            if (await userRepository.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user is null)
            {
                return BadRequest("No user found");
            }
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
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
            if (await userRepository.SaveAllAsync())
            {
                return Ok();
            }
            return BadRequest("Failed to delete photo");
        }
    }
}
