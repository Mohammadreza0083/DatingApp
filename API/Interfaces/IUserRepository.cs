using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IUserRepository
{
    /// <summary>
    /// Update user entity data
    /// </summary>
    /// <param name="user"></param>
    void Update(AppUsers user);
    /// <summary>
    /// Get a list of all users **Async Method**
    /// </summary>
    /// <returns>A IEnumerable list of Users</returns>
    Task<IEnumerable<AppUsers>> GetUsersAsync();
    /// <summary>
    /// Get a user by id **Async Method**
    /// </summary>
    /// <param name="id"></param>
    /// <returns>User or Null</returns>
    Task<AppUsers?> GetUserByIdAsync(int id);
    /// <summary>
    /// Get a user by username **Async Method**
    /// </summary>
    /// <param name="username"></param>
    /// <returns>User or Null</returns>
    Task<AppUsers?> GetUserByUsernameAsync(string username);
    /// <summary>
    /// Get a list of users **Async Method**
    /// </summary>
    /// <returns>list of users</returns>
    Task<PagedList<MembersDto>> GetAllMembersAsync(UserParams userParams);

    /// <summary>
    /// Get user by username **Async Method** 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="isCurrentUser"></param>
    /// <returns>user</returns>
    Task<MembersDto?> GetMemberAsync(string username, bool isCurrentUser);
    
    Task<AppUsers?> AddUserAsync(RegisterDto registerDto);
    
    Task<AppUsers?> GetUserByPhotoIdAsync(int photoId);
}