using API.DTO;
using API.Entities;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<UserLike?> GetUserLikeAsync(int sourceUserId, int likedUserId);
    
    Task<IEnumerable<MembersDto>> GetUserLikesAsync(string predicate, int userId);
    
    Task<IEnumerable<int>> GetCurrentUserLikeIdsAsync(int userId);
    
    Task<bool> SaveChangesAsync();
    
    void DeleteUserLike(UserLike userLike);
    
    void AddUserLike(UserLike userLike);
}