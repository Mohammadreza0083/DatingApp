using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Repository for managing user-related operations
/// Implements IUserRepository interface
/// </summary>
public class UserRepository(UserManager<AppUsers> manager, DataContext context, IMapper mapper): IUserRepository
{
    /// <summary>
    /// Updates the state of a user entity in the database
    /// Note: This method is unnecessary as Entity Framework automatically tracks changes
    /// </summary>
    /// <param name="user">The user entity to update</param>
    public void Update(AppUsers user)
    {
        context.Entry(user).State = EntityState.Modified;
    }

    /// <summary>
    /// Retrieves all users from the database
    /// Includes user photos to handle Entity Framework lazy loading
    /// </summary>
    /// <returns>An IEnumerable collection of users</returns>
    public async Task<IEnumerable<AppUsers>> GetUsersAsync()
    {
        List<AppUsers> users = await context.Users
            .Include(u => u.Photos)
            .ToListAsync();
        return users;
    }

    /// <summary>
    /// Retrieves a user by their ID
    /// Uses Entity Framework's FirstOrDefaultAsync for efficient querying
    /// Includes user photos to handle Entity Framework lazy loading
    /// </summary>
    /// <param name="id">The ID of the user to find</param>
    /// <returns>The user if found, null otherwise</returns>
    public async Task<AppUsers?> GetUserByIdAsync(int id)
    {
        AppUsers? user = await context.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.Id == id);
        return user;
    }

    /// <summary>
    /// Retrieves a user by their username
    /// Uses Entity Framework's SingleOrDefaultAsync as usernames are unique
    /// Includes user photos to handle Entity Framework lazy loading
    /// </summary>
    /// <param name="username">The username to search for</param>
    /// <returns>The user if found, null otherwise</returns>
    public async Task<AppUsers?> GetUserByUsernameAsync(string username)
    {
        AppUsers? user = await context.Users
            .Include(u => u.Photos)
            .SingleOrDefaultAsync(u => u.NormalizedUserName == username.ToUpper());
        return user;
    }

    /// <summary>
    /// Retrieves a paginated list of members based on specified parameters
    /// Applies filtering based on gender, age range, and sorting preferences
    /// </summary>
    /// <param name="userParams">Parameters for filtering and pagination</param>
    /// <returns>A paginated list of MemberDto objects</returns>
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
        
        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(u => u.Created),
            _ => query.OrderByDescending(u => u.LastActivity)
        };
        
        return await PagedList<MembersDto>.CreateAsync(query.ProjectTo<MembersDto>(mapper.ConfigurationProvider),
            userParams.PageNum, userParams.PageSize);
    }

    /// <summary>
    /// Retrieves a member by username and maps to MemberDto
    /// Optionally ignores query filters for the current user
    /// </summary>
    /// <param name="username">The username to search for</param>
    /// <param name="isCurrentUser">Whether the requested user is the current user</param>
    /// <returns>The mapped MemberDto if found, null otherwise</returns>
    public async Task<MembersDto?> GetMemberAsync(string username, bool isCurrentUser)
    {
        var query= context.Users
            .Where(u => u.NormalizedUserName == username.ToUpper())
            .ProjectTo<MembersDto>(mapper.ConfigurationProvider)
            .AsQueryable();
        if (isCurrentUser)
        {
            query = query.IgnoreQueryFilters();
        }
        return await query.SingleOrDefaultAsync();
    }

    /// <summary>
    /// Adds a new user to the database
    /// Validates username uniqueness and creates user with provided credentials
    /// </summary>
    /// <param name="registerDto">The registration data for the new user</param>
    /// <returns>The created user if successful, null otherwise</returns>
    /// <exception cref="Exception">Thrown if username is taken or user creation fails</exception>
    public async Task<AppUsers?> AddUserAsync(RegisterDto registerDto)
    {
        if (await context.Users.SingleOrDefaultAsync(u => u.NormalizedUserName == registerDto.Username.ToUpper()) 
            is not null)
        {
            throw new Exception("Username is taken");
        }
        var user = mapper.Map<AppUsers>(registerDto);
        user.UserName = registerDto.Username.ToLower();
        var result = await manager.CreateAsync(user, registerDto.Password);
        if (result.Succeeded)
        {
            return user;
        }
        foreach (var error in result.Errors)
        {
            throw new Exception(error.Description);
        }
        return null;
    }

    /// <summary>
    /// Retrieves a user by their photo ID
    /// Includes user photos and ignores query filters
    /// </summary>
    /// <param name="photoId">The ID of the photo to search for</param>
    /// <returns>The user who owns the photo if found, null otherwise</returns>
    public async Task<AppUsers?> GetUserByPhotoIdAsync(int photoId)
    {
        return await context.Users
            .Include(p => p.Photos)
            .IgnoreQueryFilters()
            .Where(u => u.Photos.Any(p => p.Id == photoId))
            .SingleOrDefaultAsync();
    }
}