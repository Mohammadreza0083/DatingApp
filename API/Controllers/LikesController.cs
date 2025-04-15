using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller for managing user likes and matches
/// Handles like operations and retrieval of liked users
/// </summary>
public class LikesController(IUnitOfWork repo) : BaseApiController
{
    /// <summary>
    /// Toggles a like between two users
    /// If the like doesn't exist, it creates one; if it exists, it removes it
    /// </summary>
    /// <param name="targetUserId">ID of the user to like/unlike</param>
    /// <returns>Ok if successful, BadRequest if failed</returns>
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();

        if (sourceUserId == targetUserId)
        {
            return BadRequest("You cannot like yourself");
        }
        
        var existingLike = await repo.LikesRepository.GetUserLikeAsync(sourceUserId, targetUserId);

        if (existingLike is null)
        {
            UserLike like = new()
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };
            
            repo.LikesRepository.AddUserLike(like);
        }
        else
        {
            repo.LikesRepository.DeleteUserLike(existingLike);
        }

        if (await repo.Complete())
        {
            return Ok();
        }
        
        return BadRequest("Failed to like user");
    }

    /// <summary>
    /// Retrieves the list of user IDs that the current user has liked
    /// </summary>
    /// <returns>List of user IDs that the current user has liked</returns>
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        return Ok(
            await repo.LikesRepository.GetCurrentUserLikeIdsAsync(User.GetUserId()));
    }

    /// <summary>
    /// Retrieves a paginated list of users that the current user has liked
    /// </summary>
    /// <param name="likesParams">Pagination and filtering parameters</param>
    /// <returns>Paginated list of member DTOs with pagination information</returns>
    [HttpGet]
    public async Task<ActionResult<PagedList<MembersDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        
        var users = await repo.LikesRepository.GetUserLikesAsync(likesParams);
        
        Response.AddPaginationHeader(users);
        
        return Ok(users);
    }
}