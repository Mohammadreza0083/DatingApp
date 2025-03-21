using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(ILikesRepository repo) : BaseApiController
{
    [HttpPost("{targetUserId:int}")]

    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();

        if (sourceUserId == targetUserId)
        {
            return BadRequest("You cannot like yourself");
        }
        
        var existingLike = await repo.GetUserLikeAsync(sourceUserId, targetUserId);

        if (existingLike is null)
        {
            UserLike like = new()
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };
            
            repo.AddUserLike(like);
        }
        else
        {
            repo.DeleteUserLike(existingLike);
        }

        if (await repo.SaveChangesAsync())
        {
            return Ok();
        }
        
        return BadRequest("Failed to like user");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        return Ok(
            await repo.GetCurrentUserLikeIdsAsync(User.GetUserId()));
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MembersDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        
        var users = await repo.GetUserLikesAsync(likesParams);
        
        Response.AddPaginationHeader(users);
        
        return Ok(users);
    }
}