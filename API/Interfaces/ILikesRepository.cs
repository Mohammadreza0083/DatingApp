using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

/// <summary>
/// Interface for managing user likes and relationships
/// </summary>
public interface ILikesRepository
{
    /// <summary>
    /// Gets a specific user like relationship
    /// </summary>
    /// <param name="sourceUserId">The ID of the user who liked</param>
    /// <param name="likedUserId">The ID of the user who was liked</param>
    /// <returns>The UserLike entity if found, null otherwise</returns>
    Task<UserLike?> GetUserLikeAsync(int sourceUserId, int likedUserId);
    
    /// <summary>
    /// Gets a paginated list of users that a specific user has liked or been liked by
    /// </summary>
    /// <param name="likesParams">Parameters for filtering and pagination</param>
    /// <returns>A paginated list of member DTOs</returns>
    Task<PagedList<MembersDto>> GetUserLikesAsync(LikesParams likesParams);
    
    /// <summary>
    /// Gets the IDs of users that a specific user has liked
    /// </summary>
    /// <param name="userId">The ID of the user whose likes are being queried</param>
    /// <returns>A list of user IDs</returns>
    Task<IEnumerable<int>> GetCurrentUserLikeIdsAsync(int userId);
    
    /// <summary>
    /// Deletes a user like relationship
    /// </summary>
    /// <param name="userLike">The UserLike entity to delete</param>
    void DeleteUserLike(UserLike userLike);
    
    /// <summary>
    /// Adds a new user like relationship
    /// </summary>
    /// <param name="userLike">The UserLike entity to add</param>
    void AddUserLike(UserLike userLike);
}