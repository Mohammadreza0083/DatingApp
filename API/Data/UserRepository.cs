using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository(DataContext context, IMapper mapper): IUserRepository
{
    /// <summary>
    /// Unnecessary method
    /// EntityFramework flow data if anything change
    /// </summary>
    /// <param name="user"></param>
    public void Update(AppUsers user)
    {
        context.Entry(user).State = EntityState.Modified;
    }

    /// <summary>
    /// Save all changes in DB
    /// </summary>
    /// <returns>1(true) if anything change in DB and 0(false) if nothing change in DB</returns>
    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Get all user in database
    /// Include users photo because EntityFramework is lazy and cant find users photo
    /// </summary>
    /// <returns>A IEnumerable list of users in database</returns>
    public async Task<IEnumerable<AppUsers>> GetUsersAsync()
    {
        List<AppUsers> users = await context.Users
            .Include(u => u.Photos)
            .ToListAsync();
        return users;
    }

    /// <summary>
    /// Find user by id
    /// use Linq (FirstOrDefaultAsync)
    /// Include user photo because EntityFramework is lazy and cant find user photo
    /// Async Method
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// User is find
    /// Null is user notfound
    /// </returns>
    public async Task<AppUsers?> GetUserByIdAsync(int id)
    {
        AppUsers? user = await context.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.Id == id);
        return user;
    }

    /// <summary>
    /// Find user by username
    /// Use Linq (SingleOrDefaultAsync) we have unique username in DB
    /// Include user photo because EntityFramework is lazy and cant find user photo
    /// Async Method
    /// </summary>
    /// <param name="username"></param>
    /// <returns>
    /// User if found
    /// Null if notfound
    /// </returns>
    public async Task<AppUsers?> GetUserByUsernameAsync(string username)
    {
        AppUsers? user = await context.Users
            .Include(u => u.Photos)
            .SingleOrDefaultAsync(u => u.UserName == username);
        return user;
    }

    /// <summary>
    /// Map users to members
    /// </summary>
    /// <returns>list of MemberDto or Null</returns>
    public async Task<PagedList<MembersDto>> GetAllMembersAsync(UserParams userParams)
    {
        var query = context.Users.AsQueryable();
        
        query = query.Where(u=> u.UserName != userParams.CurrentUsername);

        if (userParams.Gender != null)
        {
            query = query.Where(u => u.Gender == userParams.Gender);
        }
        
        var minDob = DateOnly.FromDateTime(DateTime.Now.AddYears(-userParams.MaxAge - 1));
        
        var maxDob = DateOnly.FromDateTime(DateTime.Now.AddYears(-userParams.MinAge));
        
        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
        
        return await PagedList<MembersDto>.CreateAsync(query.ProjectTo<MembersDto>(mapper.ConfigurationProvider),
            userParams.PageNum, userParams.PageSize);
    }

    /// <summary>
    /// Map user to MembersDto 
    /// </summary>
    /// <param name="username"></param>
    /// <returns>MemberDto or Null</returns>
    public async Task<MembersDto?> GetMemberAsync(string username)
    {
        MembersDto? membersDto = await context.Users
            .Where(u => u.UserName == username)
            .ProjectTo<MembersDto>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        return membersDto;
    }

    public async Task<bool> AddUserAsync(AppUsers user)
    {
        await context.Users.AddAsync(user);
        if (await context.Users.SingleOrDefaultAsync(u => u.UserName == user.UserName) 
            is not null)
        {
            return false;
        }

        await context.Users.AddAsync(user);
        return await context.SaveChangesAsync() > 0;
    }
}