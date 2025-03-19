using System.Security.Cryptography;
using System.Text;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountController(IUserRepository userRepository, 
    ITokenServices tokenServices, IMapper mapper)
    : BaseApiController
{
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userRepository.GetUserByUsernameAsync(loginDto.Username);
        if (user == null)
            return Unauthorized("Invalid username");
        using var hmc = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmc.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for (var i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
                return Unauthorized("Invalid password");
        }
        return new UserDto
        {
            Username = user.UserName, 
            KnownAs = user.KnownAs,
            Token = tokenServices.CreateToken(user),
            Gender = user.Gender,
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        using var hmc = new HMACSHA512();

        var user = mapper.Map<AppUsers>(registerDto);
        user.UserName = registerDto.Username.ToLower();
        user.PasswordHash = hmc.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmc.Key;

        if (await userRepository.AddUserAsync(user))
        {
            return new UserDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Token = tokenServices.CreateToken(user),
                Gender = user.Gender
            };
        }
        
        return BadRequest("Username is taken");
    }
}
