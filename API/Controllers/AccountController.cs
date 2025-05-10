using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller for handling user authentication and registration
/// Manages user login, registration, and token generation
/// </summary>
public class AccountController(IUserRepository userRepository, 
    ITokenServices tokenServices,UserManager<AppUsers> userManager)
    : BaseApiController
{
    /// <summary>
    /// Authenticates a user and generates a JWT token
    /// </summary>
    /// <param name="loginDto">User credentials (username and password)</param>
    /// <returns>User DTO with token if successful, Unauthorized if credentials are invalid</returns>
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userRepository.GetUserByUsernameAsync(loginDto.Username);
        if (user is null || user.UserName is null)
            return Unauthorized("Invalid username");
        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result)
            return Unauthorized("Invalid password");
        return new UserDto
        {
            Username = user.UserName, 
            KnownAs = user.KnownAs,
            Token = await tokenServices.CreateToken(user),
            Gender = user.Gender,
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
    }

    /// <summary>
    /// Registers a new user in the system
    /// </summary>
    /// <param name="registerDto">User registration information</param>
    /// <returns>User DTO with token if successful, BadRequest if registration fails</returns>
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        var user = await userRepository.AddUserAsync(registerDto);
        if (user is not  null && user.UserName is not null)
        {
            return new UserDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Token = await tokenServices.CreateToken(user),
                Gender = user.Gender
            };
        }
        return BadRequest("Something went wrong");
    }
}
