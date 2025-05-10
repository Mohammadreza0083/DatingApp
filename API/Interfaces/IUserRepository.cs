using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

/// <summary>
/// Interface for managing user data and operations
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Updates a user entity in the repository
    /// </summary>
    /// <param name="user">The user entity to update</param>
    void Update(AppUsers user);

    /// <summary>
    /// Gets all users from the repository
    /// </summary>
    /// <returns>A list of all users</returns>
    Task<IEnumerable<AppUsers>> GetUsersAsync();

    /// <summary>
    /// Gets a user by their ID
    /// </summary>
    /// <param name="id">The ID of the user to retrieve</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<AppUsers?> GetUserByIdAsync(int id);

    /// <summary>
    /// Gets a user by their username
    /// </summary>
    /// <param name="username">The username of the user to retrieve</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<AppUsers?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Gets a paginated list of member DTOs
    /// </summary>
    /// <param name="userParams">Parameters for filtering and pagination</param>
    /// <returns>A paginated list of member DTOs</returns>
    Task<PagedList<MembersDto>> GetAllMembersAsync(UserParams userParams);

    /// <summary>
    /// Gets a member DTO by username
    /// </summary>
    /// <param name="username">The username of the member to retrieve</param>
    /// <param name="isCurrentUser">Whether the requested member is the current user</param>
    /// <returns>The member DTO if found, null otherwise</returns>
    Task<MembersDto?> GetMemberAsync(string username, bool isCurrentUser);
    
    /// <summary>
    /// Adds a new user to the repository
    /// </summary>
    /// <param name="registerDto">The registration data for the new user</param>
    /// <returns>The created user if successful, null otherwise</returns>
    Task<AppUsers?> AddUserAsync(RegisterDto registerDto);
    
    /// <summary>
    /// Gets a user by their photo ID
    /// </summary>
    /// <param name="photoId">The ID of the photo</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<AppUsers?> GetUserByPhotoIdAsync(int photoId);
}