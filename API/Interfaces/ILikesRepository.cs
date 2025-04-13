using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<UserLike?> GetUserLikeAsync(int sourceUserId, int likedUserId);
    
    Task<PagedList<MembersDto>> GetUserLikesAsync(LikesParams likesParams);
    
    Task<IEnumerable<int>> GetCurrentUserLikeIdsAsync(int userId);
    
    void DeleteUserLike(UserLike userLike);
    
    void AddUserLike(UserLike userLike);
}