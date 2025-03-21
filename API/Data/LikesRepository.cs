using API.DTO;
using API.Entities;
using API.Helpers;
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