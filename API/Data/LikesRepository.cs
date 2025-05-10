using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Repository for managing user likes and relationships
/// Implements ILikesRepository interface
/// </summary>
public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    /// <summary>
    /// Retrieves a like relationship between two users
    /// </summary>
    /// <param name="sourceUserId">The ID of the user who liked</param>
    /// <param name="likedUserId">The ID of the user who was liked</param>
    /// <returns>The UserLike entity if found, null otherwise</returns>
    public async Task<UserLike?> GetUserLikeAsync(int sourceUserId, int likedUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, likedUserId);
    }

    /// <summary>
    /// Retrieves a paginated list of users that a user has liked or been liked by
    /// </summary>
    /// <param name="likesParams">Parameters for filtering and pagination</param>
    /// <returns>A paginated list of MembersDto objects</returns>
    public async Task<PagedList<MembersDto>> GetUserLikesAsync(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();

        IQueryable<MembersDto> query;

        switch (likesParams.Predicate)
        {
            case "liked":
                query = likes
                    .Where(s => s.SourceUserId == likesParams.UserId)
                    .Select(t => t.TargetUser)
                    .ProjectTo<MembersDto>(mapper.ConfigurationProvider);
                break;
            case "likedBy":
                query = likes
                    .Where(t => t.TargetUserId == likesParams.UserId)
                    .Select(s => s.SourceUser)
                    .ProjectTo<MembersDto>(mapper.ConfigurationProvider);
                break;
            default:
                var likeIds = await GetCurrentUserLikeIdsAsync(likesParams.UserId);

                query = likes
                    .Where(u => u.TargetUserId == likesParams.UserId && likeIds.Contains(u.SourceUserId))
                    .Select(t => t.SourceUser)
                    .ProjectTo<MembersDto>(mapper.ConfigurationProvider);
                break;
        }

        return await PagedList<MembersDto>.CreateAsync(query, likesParams.PageNum, likesParams.PageSize);
    }

    /// <summary>
    /// Retrieves a list of user IDs that the current user has liked
    /// </summary>
    /// <param name="userId">The ID of the current user</param>
    /// <returns>A list of user IDs</returns>
    public async Task<IEnumerable<int>> GetCurrentUserLikeIdsAsync(int userId)
    {
        return await context.Likes
            .Where(s => s.SourceUserId == userId)
            .Select(t => t.TargetUserId)
            .ToListAsync();
    }

    /// <summary>
    /// Deletes a like relationship from the database
    /// </summary>
    /// <param name="userLike">The UserLike entity to delete</param>
    public void DeleteUserLike(UserLike userLike)
    {
        context.Likes.Remove(userLike);
    }

    /// <summary>
    /// Adds a new like relationship to the database
    /// </summary>
    /// <param name="userLike">The UserLike entity to add</param>
    public void AddUserLike(UserLike userLike)
    {
        context.Likes.Add(userLike);
    }
}