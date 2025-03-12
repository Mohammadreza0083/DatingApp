using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenServices tokenServices)
    : BaseApiController
{
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(x =>
                x.UserName == loginDto.Username.ToLower()
        );
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
            Token = tokenServices.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username))
            return BadRequest("Username is taken");
        return Ok();
        // using var hmc = new HMACSHA512();
        // AppUsers user = new()
        // {
        //     UserName = registerDto.Username.ToLower(),
        //     PasswordHash = hmc.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        //     PasswordSalt = hmc.Key,
        // };
        // context.Users.Add(user);
        //
        // await context.SaveChangesAsync();
        // return new UserDto { Username = user.UserName, Token = tokenServices.CreateToken(user) };
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}
