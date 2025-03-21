using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    public async Task<UserLike?> GetUserLikeAsync(int sourceUserId, int likedUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, likedUserId);
    }

    public async Task<IEnumerable<MembersDto>> GetUserLikesAsync(string predicate, int userId)
    {
        var likes = context.Likes.AsQueryable();

        switch (predicate)
        {
            case "liked":
                return await likes
                    .Where(s=> s.SourceUserId == userId)
                    .Select(t => t.TargetUser)
                    .ProjectTo<MembersDto>(mapper.ConfigurationProvider)
                    .ToListAsync();
            case "likedBy":
                return await likes
                    .Where(t=> t.TargetUserId == userId)
                    .Select(s => s.SourceUser)
                    .ProjectTo<MembersDto>(mapper.ConfigurationProvider)
                    .ToListAsync();
            default:
                var likeIds = await GetCurrentUserLikeIdsAsync(userId);
                
                return await likes 
                    .Where(u => u.TargetUserId == userId && likeIds.Contains(u.SourceUserId))
                    .Select(t => t.SourceUser)
                    .ProjectTo<MembersDto>(mapper.ConfigurationProvider)
                    .ToListAsync();
        }
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIdsAsync(int userId)
    {
        return await context.Likes
            .Where(s => s.SourceUserId == userId)
            .Select(t => t.TargetUserId)
            .ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void DeleteUserLike(UserLike userLike)
    {
        context.Likes.Remove(userLike);
    }

    public void AddUserLike(UserLike userLike)
    {
        context.Likes.Add(userLike);
    }
}